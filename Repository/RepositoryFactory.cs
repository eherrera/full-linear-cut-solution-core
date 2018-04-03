using LinealCutOptimizer.Core.Repository.Implementation;

namespace LinealCutOptimizer.Core.Repository
{
    public interface IRepositoryFactory
    {
        IMesurementRepository CreateMesurementRepository();
        IParamsRepository CreateParamsRepository();
        IBarPatternRepository CreateBarPatternRepository();
    }

    public class RepositoryFactory : IRepositoryFactory
    {
        private static IRepositoryFactory _self;

        private RepositoryFactory()
        {
            
        }

        public static IRepositoryFactory GetInstance()
        {
            return _self ?? (_self = new RepositoryFactory());
        }

        public IMesurementRepository CreateMesurementRepository()
        {
            return new MeasurementRepository();
        }

        public IParamsRepository CreateParamsRepository()
        {
            return new ParamsRepository();
        }

        public IBarPatternRepository CreateBarPatternRepository()
        {
            return new BarPatternRepository();
        }
    }
}