using System.Collections.Generic;
using System.Linq;
using LinealCutOptimizer.Core.Model;

namespace FullLinearCutSolution.Core.Helpers
{
    public static class HtmlHelper
    {
        public static string ToHtml(this IList<Summary> result)
        {
            var html = "<body>";
            foreach (var summary in result)
            {
                html += $"<h1>Grupo: {summary.GroupName}</h1><hr>";
                html += RenderSummaryTable(summary);
            }
            html += "</body>";
            return html;
        }

        private static string RenderSummaryTable(Summary summary)
        {
            var html = "";

            foreach (var sol in summary.SolutionsByReference)
            {
                html += $"<h2>Diámetro: {sol.Key}</h2><hr>";

                html += "<table style='border:1px solid'>";
                html +=
                    "<tr><th style='border:1px solid'>Corte x Barras</th><th style='border:1px solid'>Long. Barra</th><th style='border:1px solid'>Cant. Barras</th><th style='border:1px solid'>Resto</th></tr>";
                foreach (var concretSol in sol.Value)
                {
                    html += "<tr>";

                    //corte por barras
                    html += "<td style='border:1px solid'>";
                    html += concretSol.CutsRepresentation;
                    html += "</td>";
                    //longitud de la barra
                    html += "<td style='border:1px solid'>";
                    html += concretSol.GetBar().Length;
                    html += "</td style='border:1px solid'>";
                    //cant la barra
                    html += "<td style='border:1px solid'>";
                    html += concretSol.BarCount;
                    html += "</td>";
                    //resto
                    html += "<td style='border:1px solid'>";
                    html += concretSol.Waste;
                    html += "</td>";

                    html += "</tr>";
                }
                html += "</table>";
                html += $"<h3>Total. Barras: {sol.Value.Sum(x => x.BarCount)}</h3>";
            }


            return html;
        }
    }
}