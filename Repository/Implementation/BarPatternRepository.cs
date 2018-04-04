using System.Collections.Generic;
using System.Linq;
using FullLinearCutSolution.Core;

namespace LinealCutOptimizer.Core.Repository.Implementation
{
    internal class BarPatternRepository : BaseRepository, IBarPatternRepository
    {
        public IList<BarPattern> GetAll()
        {
            IList<BarPattern> result = DbContext.BarPatterns.ToList();
            return result;
        }
    }
}