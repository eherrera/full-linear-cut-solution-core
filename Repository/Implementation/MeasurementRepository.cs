using System.Collections.Generic;
using System.Linq;
using FullLinearCutSolution.Core;

namespace LinealCutOptimizer.Core.Repository.Implementation
{
    internal class MeasurementRepository : BaseRepository, IMesurementRepository
    {
        public IList<MeasurementUnit> GetAll()
        {
            IList<MeasurementUnit> result = DbContext.MeasurementUnit.ToList();
            return result;
        }
    }
}