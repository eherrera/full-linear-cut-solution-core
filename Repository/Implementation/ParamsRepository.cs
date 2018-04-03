using System.Data.Entity;
using System.Linq;

namespace LinealCutOptimizer.Core.Repository.Implementation
{
    internal class ParamsRepository : BaseRepository, IParamsRepository
    {
        public Params Get(bool includeDeps = true)
        {
            if (includeDeps)
            {
                return DbContext.Params.Include(nameof(Params.MeasurementUnit)).FirstOrDefault();
            }

            return DbContext.Params.FirstOrDefault();
        }

        public Params BeginUpdate(Params pParams)
        {
            using (var context = new EFLinearCutSolutionEntities())
            {
                var attachedEntity = context.Params.Local.FirstOrDefault(p => p.Id == pParams.Id);
                if (attachedEntity != null) return attachedEntity;
                context.Entry(pParams).State = EntityState.Modified;
                return pParams;
            }
        }
    }
}