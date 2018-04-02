using FullLinearCutSolution.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using LinealCutOptimizer.Core.Model;

namespace FullLinearCutSolution.Core
{
    public class Optimizer
    {
        public List<CutSolution> Optimize(Bar bar, Order order, OptimizerStrategy strategy = OptimizerStrategy.Optimize)
        {
            if (bar == null || order == null)
            {
                throw new ArgumentException("Debe especificar una orden válida.");
            }
            if (!ValidateOrder(bar, order))
            {
                throw new Exception("Exiten medidas de los elementos de la orden que exceden la longitud de la barra.");
            }

            switch (strategy)
            {
                case OptimizerStrategy.Optimize:
                    throw new ArgumentOutOfRangeException(nameof(strategy), strategy,
                        "The Optimize strategy is not supported in this version");
                case OptimizerStrategy.Traditional:
                    return SolveUsingTraditionalStrategy(bar, order);
                default:
                    throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
            }
        }

        private static List<CutSolution> SolveUsingTraditionalStrategy(Bar bar, Order order)
        {
            var result = new List<CutSolution>();
            order.Normalize();
            order.Sort();
            while (order.UnappliedItems.Any())
            {
                var currentItem = order.UnappliedItems[0];
                while (!currentItem.Applied)
                {
                    if (result.Any(s => s.Waste > 0))
                    {
                        var maxMeasurementWaste = decimal.MinValue;
                        CutSolution candidateSolution = null;
                        foreach (var s in result)
                        {
                            var avaiable = s.WasteDictionary.Where(w => w.Value > 0).ToList();
                            if (!avaiable.Any())
                            {
                                continue;
                            }
                            var max = avaiable.Max(d => d.Key);
                            if (max > maxMeasurementWaste)
                            {
                                maxMeasurementWaste = max;
                                candidateSolution = s;
                            }
                        }
                        if (maxMeasurementWaste >= currentItem.Measurement)
                        {
                            var newBar = new Bar {Length = maxMeasurementWaste};
                            var maxUnits = candidateSolution?.WasteDictionary
                                .FirstOrDefault(d => d.Key == maxMeasurementWaste).Value;
                            if (maxUnits > 0)
                            {
                                var innerSol = BuildSolution(newBar, order, currentItem, maxUnits);
                                if (innerSol != null)
                                {
                                    result.Add(innerSol);
                                    candidateSolution.SubSolutions.Add(innerSol);
                                    innerSol.ParentSolutionId = candidateSolution.Id;
                                    continue;
                                }
                            }
                        }
                    }
                    var sol = BuildSolution(bar, order, currentItem);
                    if (sol != null)
                    {
                        result.Add(sol);
                    }
                }
            }

            return result;
        }

        private static CutSolution BuildSolution(Bar bar, Order order, OrderItem currentItem, int? maxUnits = null)
        {
            var maxPatternLength = Math.Floor(bar.Length / currentItem.Measurement);
            var unappliedUnits = currentItem.Units - currentItem.AppliedUnits;
            maxPatternLength = maxPatternLength > unappliedUnits ? unappliedUnits : maxPatternLength;
            var pattern = new CutPattern();
            for (var i = 0; i < maxPatternLength; i++)
            {
                pattern.Measurements.Add(currentItem.Measurement);
            }
            if (pattern.TrySatisfy(order.UnappliedItems, false, maxUnits))
            {
                var solution = new CutSolution(bar);
                solution.SetPattern(pattern);
                return solution;
            }
            return null;
        }

        private static List<CutSolution> SolveUsingTraditionalStrategyOld(Bar bar, Order order)
        {
            var result = new List<CutSolution>();
            order.Normalize();
            order.Sort();

            while (order.UnappliedItems.Any())
            {
                var patterns = new List<CutPattern>();
                foreach (var item in order.UnappliedItems)
                {
                    var maxPatternLength = Math.Floor(bar.Length / item.Measurement);
                    var unapplied = item.Units - item.AppliedUnits;
                    maxPatternLength = maxPatternLength > unapplied ? unapplied : maxPatternLength;
                    var pattern = new CutPattern();
                    for (var i = 0; i < maxPatternLength; i++)
                    {
                        pattern.Measurements.Add(item.Measurement);
                    }
                    patterns.Add(pattern);
                }
                patterns.Sort(OptimizerStrategy.Optimize);
                foreach (var pattern in patterns)
                {
                    while (true)
                    {
                        if (pattern.Measurements.Any() && pattern.TrySatisfy(order.UnappliedItems))
                        {
                            while (true)
                            {
                                if (!pattern.Units.Any())
                                {
                                    break;
                                }
                                var units = pattern.Units[0];
                                var mCandidate =
                                    order.UnappliedItems.FirstOrDefault(
                                        i => i.Measurement <= bar.Length - pattern.Measurements.Sum());
                                if (mCandidate == null) break;
                                if (pattern.Measurements.Sum() + mCandidate.Measurement <= bar.Length)
                                {
                                    if (mCandidate.Units - mCandidate.AppliedUnits < units)
                                    {
                                        units = mCandidate.Units - mCandidate.AppliedUnits;
                                    }
                                    pattern.Measurements.Add(mCandidate.Measurement);
                                    pattern.Units.Add(units);
                                    mCandidate.AppliedUnits += units;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            var solution = new CutSolution(bar);
                            solution.SetPattern(pattern);
                            AnalizeWaste(solution, order, OptimizerStrategy.Traditional);
                            result.Add(solution);
                            break;
                        }
                        if (pattern.Measurements.Count > 0)
                        {
                            pattern.Measurements.RemoveAt(0);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }


            return result;
        }

        /*private static List<CutSolution> SolveUsingOptimizeStrategy(Bar bar, Order order)
        {
            var result = new List<CutSolution>();

            List<OrderItem> unappliedItems;
            // start always finding the best cases
            var forceBestCase = true;
            order.Normalize();
            order.Sort();
            var combinationLowerIndex = 1;
            do
            {
                unappliedItems = order.UnappliedItems;
                if (unappliedItems.Count == 0)
                {
                    break;
                }
                var unappliedMeasurements = unappliedItems.Select(u => u.Measurement).ToList();

                var minMeasurement = unappliedMeasurements.Min();
                var maxCombinationLength = Convert.ToInt32(bar.Length / minMeasurement);
                var to = unappliedMeasurements.Count > maxCombinationLength
                    ? unappliedMeasurements.Count
                    //@fixme find the best to
                    : maxCombinationLength / 2;

                if (forceBestCase && combinationLowerIndex <= to || !forceBestCase && combinationLowerIndex >= 0)
                {
                    var combinations = new Combinations<decimal>(unappliedMeasurements.ToList(),
                        forceBestCase ? combinationLowerIndex++ : combinationLowerIndex--,
                        GenerateOption.WithRepetition);
                    foreach (var combination in combinations)
                    {
                        var combinationSum = combination.Sum();
                        //finding exact combinations
                        if (forceBestCase && combinationSum != bar.Length)
                        {
                            continue;
                        }
                        //finding partial combinations
                        if (!forceBestCase && combinationSum > bar.Length)
                        {
                            continue;
                        }
                        var pattern = new CutPattern {Measurements = combination.ToList()};
                        if (!pattern.TrySatisfy(unappliedItems, forceBestCase)) continue;
                        var solution = new CutSolution(bar);
                        solution.SetPattern(pattern);
                        result.Add(solution);
                    }
                }
                else
                {
                    combinationLowerIndex = to;
                    forceBestCase = false;
                }
            } while (unappliedItems.Count > 0);


            return result;
        }*/

        private static void AnalizeWaste(CutSolution solution, Order order, OptimizerStrategy strategy)
        {
            while (true)
            {
                var pattern = solution.GetCutPattern();
                if (pattern == null) return;
                var unappliedItems = order.UnappliedItems;
                if (unappliedItems.Count == 0) return;
                var minMeasurement = unappliedItems.Min(i => i.Measurement);
                if (solution.WasteDictionary.All(w => w.Key < minMeasurement)) return;
                foreach (var waste in solution.WasteDictionary)
                {
                    var mCandidate =
                        order.Items.FirstOrDefault(i => i.Measurement <= waste.Key && i.Units - i.AppliedUnits > 0);
                    if (mCandidate == null) continue;
                    var avaiableAmount = waste.Key * waste.Value;
                    var units = Convert.ToInt32(Math.Floor(avaiableAmount / mCandidate.Measurement));
                    var unappliedUnits = mCandidate.Units - mCandidate.AppliedUnits;
                    if (units > unappliedUnits)
                    {
                        units = unappliedUnits;
                    }
                    if (units <= 0) break;
                    pattern.Measurements.Add(mCandidate.Measurement);
                    pattern.Units.Add(units);
                    mCandidate.AppliedUnits += units;
                }
            }
        }

        private static bool ValidateOrder(Bar bar, Order order)
        {
            return order.Items.All(i => i.Measurement <= bar.Length);
        }
    }
}