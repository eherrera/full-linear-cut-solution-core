using System.Collections.Generic;
using LinealCutOptimizer.Core.Model;

namespace FullLinearCutSolution.Core.Helpers
{
    public static class HtmlHelper
    {
        public static string ToHtml(this IList<Summary> result)
        {
            var html = "";
            foreach (var summary in result)
            {
                html += $"<h1>Grupo: {summary.GroupName}</h1><hr>";
                html += RenderSummaryTable(summary);
            }
            return html;
        }

        private static string RenderSummaryTable(Summary summary)
        {
            var html = "";

            foreach (var sol in summary.SolutionsByReference)
            {
                html += $"<h2>Diámetro: {sol.Key}</h2><hr>";

                html += "<table>";
                html +=
                    "<tr><th>Longitud</th><th>Cantidad</th><th>Long. Barra</th><th>Corte x Barras</th><th>Resto</th></tr>";
                foreach (var concretSol in sol.Value)
                {
                    html += "<tr>";

                    //longitud de la barra
                    html += "<td>";
                    html += concretSol.GetBar().Length;
                    html += "</td>";
                    //corte por barras
                    html += "<td>";
                    html += string.Join(", ", concretSol.GetCutPattern().Measurements);
                    html += "</td>";
                    //resto
                    html += "<td>";
                    html += concretSol.Waste;
                    html += "</td>";

                    html += "</tr>";
                }
                html += "</table>";
            }

            return html;
        }
    }
}