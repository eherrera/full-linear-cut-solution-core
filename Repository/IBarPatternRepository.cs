using System.Collections.Generic;

namespace LinealCutOptimizer.Core.Repository
{
    public interface IBarPatternRepository
    {
        IList<BarPattern> GetAll();
    }
}