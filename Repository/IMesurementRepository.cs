using System.Collections.Generic;
using FullLinearCutSolution.Core;

namespace LinealCutOptimizer.Core.Repository
{
    public interface IMesurementRepository : IRepository
    {
        IList<MeasurementUnit> GetAll();
    }
}