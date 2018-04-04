using System;
using System.Collections.Generic;
using System.Linq;
using FullLinearCutSolution.Core;
using FullLinearCutSolution.Core.Model;

namespace LinealCutOptimizer.Core.Model
{
    public class Summary
    {
        public string GroupName { get; set; }

        public Dictionary<string, List<OrderItem>> LinesByReference { get; set; } =
            new Dictionary<string, List<OrderItem>>();

        public Dictionary<string, List<CutSolution>> SolutionsByReference { get; } =
            new Dictionary<string, List<CutSolution>>();

        public void GenerateCuts(IList<BarPattern> barPatterns)
        {
            var optimizer = new Optimizer();
            foreach (var linesByReference in LinesByReference)
            {
                var barPattern = barPatterns?.FirstOrDefault(b => b.Diameter == linesByReference.Key);
                if (barPattern == null)
                {
                    throw new Exception($"No se encontró un patrón de barra de diámetro: {linesByReference.Key}");
                }
                var bar = new Bar {Diameter = barPattern.Diameter, Length = barPattern.Length};

                var solutions = optimizer.Optimize(bar, new Order {Items = linesByReference.Value});
                SolutionsByReference.Add(linesByReference.Key, solutions);
            }
        }
    }

    public static class SummaryBuilder
    {
        public static IList<Summary> BuildFromOrderLines(List<OrderItem> orderLines, IList<BarPattern> barPatterns)
        {
            IList<Summary> summaries = new List<Summary>();
            foreach (var orderLine in orderLines)
            {
                var summary = summaries.FirstOrDefault(s => s.GroupName == orderLine.Group);
                if (summary == null)
                {
                    summary = new Summary {GroupName = orderLine.Group};
                    summaries.Add(summary);
                }

                if (!summary.LinesByReference.ContainsKey(orderLine.Reference))
                {
                    summary.LinesByReference.Add(orderLine.Reference, new List<OrderItem> {orderLine});
                }
                else
                {
                    summary.LinesByReference[orderLine.Reference].Add(orderLine);
                }
            }
            foreach (var summary in summaries)
            {
                summary.GenerateCuts(barPatterns);
            }
            return summaries;
        }
    }
}