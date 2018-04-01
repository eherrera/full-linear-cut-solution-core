using FullLinearCutSolution.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Combinatorics.Collections;
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
                    return SolveUsingOptimizeStrategy(bar, order);
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

            var unappliedItems = order.UnappliedItems;
            var unappliedMeasurements = unappliedItems.Select(u => u.Measurement).ToList();
            var patterns = new List<CutPattern>();
            foreach (var m in unappliedMeasurements)
            {

            }

            return result;
        }

        private static List<CutSolution> SolveUsingOptimizeStrategy(Bar bar, Order order)
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
        }

        private void AnalizeWaste(CutSolution solution, Order order, OptimizerStrategy strategy)
        {
            if (solution.Waste > 0)
            {
                var unappliedItems = order.UnappliedItems;
                var unappliedItem = unappliedItems.FirstOrDefault(i => i.Measurement == solution.Waste);
                if (unappliedItem == null)
                {
                    return;
                }
                var pattern = solution.GetCutPattern();
                var usedBarsCountInPattern = pattern.Units.LastOrDefault();
                var units = 0;
                while (units < usedBarsCountInPattern)
                {
                    if (unappliedItem.Applied)
                    {
                        break;
                    }
                    unappliedItem.AppliedUnits++;
                    units++;
                }
                if (units > 0)
                {
                    pattern.Measurements.Add(unappliedItem.Measurement);
                    pattern.Units.Add(units);
                }
            }
        }

        private static bool ValidateOrder(Bar bar, Order order)
        {
            return order.Items.All(i => i.Measurement <= bar.Length);
        }
    }
}