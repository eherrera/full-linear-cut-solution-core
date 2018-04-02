using System.Globalization;

namespace FullLinearCutSolution.Core.Helpers
{
    public static class ExtensionsHelper
    {
        private static readonly CultureInfo Culture = CultureInfo.CurrentUICulture;    

        public static string Round2(this decimal value, int decimalValue = 2)
        {
            var format = "N" + decimalValue;        
            var result = value.ToString(format, Culture);
            var decimalIntegerValue = int.Parse(result.Substring(result.Length - decimalValue));
            return decimalIntegerValue == 0 ? result.Substring(0, result.Length - decimalValue - 1) : result;
        }
    }
}
