using System;
using System.Collections.Generic;
using System.Linq;

namespace LinealCutOptimizer.Core.Repository.Implementation
{
    internal class BarPatternRepository : BaseRepository, IBarPatternRepository
    {
        public IList<BarPattern> GetAll()
        {
            IList<BarPattern> result;
            using (var context = new EFLinearCutSolutionEntities())
            {
                result = context.BarPatterns.ToList();
            }
            return result;
        }
    }
}