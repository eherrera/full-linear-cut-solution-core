using System.Collections.Generic;
using System.Linq;

namespace LinealCutOptimizer.Core.Helpers
{
    public class ExcelLoader
    {
        public static IList<T> Load<T>(string fileName)
        {
            var book = new LinqToExcel.ExcelQueryFactory(fileName);

            var query =
                from row in book.Worksheet<T>(0)
                select row;
            return query.ToList();
        }
    }
}