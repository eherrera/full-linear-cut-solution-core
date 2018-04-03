using System.Collections.Generic;

namespace LinealCutOptimizer.Core.Repository
{
    public interface IMesurementRepository : IRepository
    {
        IList<MeasurementUnit> GetAll();
    }
}