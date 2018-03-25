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

            var result = new List<CutSolution>();
            var patterns = BuildPossiblePatterns(bar, order);
            patterns.Sort(strategy);

            List<OrderItem> unappliedItems;
            // start always finding the best cases
            var forceBestCase = strategy == OptimizerStrategy.Optimize;
            do
            {
                unappliedItems = order.UnappliedItems;
                var appliedUnitsSum = unappliedItems.Sum(i => i.AppliedUnits);
                foreach (var pattern in patterns)
                {
                    if (pattern.TrySatisfy(unappliedItems, forceBestCase))
                    {
                        var solution = new CutSolution(bar);
                        solution.SetPattern(pattern);
                        AnalizeWaste(solution, order, strategy);
                        result.Add(solution);
                        break;
                    }
                }
                unappliedItems = order.UnappliedItems;                
                if (appliedUnitsSum == unappliedItems.Sum(i => i.AppliedUnits))
                {
                    forceBestCase = false;
                }
            }
            while (unappliedItems.Count > 0);            

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

        public List<CutPattern> BuildPossiblePatterns(Bar bar, Order order)
        {
            var barLength = bar.Length;
            var strategies = new List<CutPattern>();
            order.Normalize();
            order.Sort();
            var orderMeasurements = order.Items.Select(i => i.Measurement).ToList();
            var minMeasurement = orderMeasurements.Min();
            Combine(strategies, orderMeasurements, barLength, strategies.Count, Convert.ToInt32(barLength / minMeasurement));
            return strategies;
        }

        private static void Combine(List<CutPattern> patterns, IReadOnlyList<decimal> orderLines, decimal barLength, int from, int to)
        {
            if (from > to)
            {
                return;
            }
            if (from == 0)
            {
                foreach (var m in orderLines)
                {
                    var pattern = new CutPattern { Measurements = new List<decimal> { m } };
                    TryAddPattern(pattern);
                }
            }
            else
            {
                var lastPatterns = patterns
                    .Where(s => s.Measurements.Count == from/* &&
                                s.Measurements.Sum() + Convert.ToInt32(barLength / to) <= barLength*/).ToList();
                foreach (var pattern in lastPatterns)
                {
                    foreach (var candidate in orderLines)
                    {
                        if (pattern.Measurements.Sum() + candidate > barLength) continue;
                        var patternClone = pattern.Clone();
                        patternClone.Measurements.Add(candidate);
                        TryAddPattern(patternClone);
                    }
                }
            }

            Combine(patterns, orderLines, barLength, ++from, to);

            bool TryAddPattern(CutPattern cutPattern)
            {
                if (cutPattern == null || patterns.ContainsPattern(cutPattern)) return false;
                patterns.Add(cutPattern.Clone());
                return true;
            }
        }
    }
}
