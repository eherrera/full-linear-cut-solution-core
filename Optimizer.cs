﻿using FullLinearCutSolution.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FullLinearCutSolution.Core
{
    public class Optimizer
    {
        public List<CutSolution> Optimize(Bar bar, Order order)
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
            List<OrderItem> unappliedItems;
            // start always finding the best cases
            var forceBestCase = true;
            do
            {
                unappliedItems = order.UnappliedItems;
                var appliedUnitsSum = unappliedItems.Sum(i => i.AppliedUnits);
                foreach (var pattern in patterns)
                {
                    if (pattern.TrySatisfy(unappliedItems, forceBestCase: forceBestCase))
                    {
                        var solution = new CutSolution(bar);
                        solution.SetPattern(pattern);
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
            Combine(strategies, orderMeasurements, barLength, strategies.Count, orderMeasurements.Count());
            return strategies.SortPatterns();
        }

        private void Combine(List<CutPattern> patterns, List<decimal> orderLines, decimal barLength, int from, int to)
        {
            if (from > barLength)
            {
                return;
            }
            if (from == 0)
            {
                for (int i = 0; i < orderLines.Count(); i++)
                {
                    CutPattern pattern = new CutPattern { Measurements = new List<decimal> { orderLines[i] } };
                    TryAddPattern(pattern);
                }
            }
            else
            {
                var lastPatterns = patterns.Where(s => s.Measurements.Count == from).ToList();
                foreach (var pattern in lastPatterns)
                {
                    for (int i = 0; i < orderLines.Count(); i++)
                    {
                        var candidate = orderLines[i];
                        if (pattern.Measurements.Sum() + candidate <= barLength)
                        {
                            var patternClone = pattern.Clone();
                            patternClone.Measurements.Add(candidate);
                            TryAddPattern(patternClone);
                        }
                    }
                }
            }

            Combine(patterns, orderLines, barLength, ++from, to);

            bool TryAddPattern(CutPattern cutPattern)
            {
                if (cutPattern != null && !patterns.ContainsPattern(cutPattern))
                {
                    patterns.Add(cutPattern.Clone());
                    return true;
                }
                return false;
            }
        }
    }
}
