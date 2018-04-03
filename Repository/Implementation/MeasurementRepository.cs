﻿using System.Collections.Generic;
using System.Linq;

namespace LinealCutOptimizer.Core.Repository.Implementation
{
    internal class MeasurementRepository : BaseRepository, IMesurementRepository
    {
        public IList<MeasurementUnit> GetAll()
        {
            IList<MeasurementUnit> result;
            using (var context = new EFLinearCutSolutionEntities())
            {
                result = context.MeasurementUnit.ToList();
            }
            return result;
        }
    }
}