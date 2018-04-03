using System.Data.Entity;
using System.Linq;

namespace LinealCutOptimizer.Core.Repository.Implementation
{
    internal class ParamsRepository : BaseRepository, IParamsRepository
    {
        public Params Get()
        {
            Params result;
            using (var context = new EFLinearCutSolutionEntities())
            {
                result = context.Params.Include(nameof(Params.MeasurementUnit)).FirstOrDefault();
            }
            return result;
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