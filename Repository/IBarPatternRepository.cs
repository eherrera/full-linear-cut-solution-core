using System.Collections.Generic;
using FullLinearCutSolution.Core;

namespace LinealCutOptimizer.Core.Repository
{
    public interface IBarPatternRepository
    {
        IList<BarPattern> GetAll();
    }
}