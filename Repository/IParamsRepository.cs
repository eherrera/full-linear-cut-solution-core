namespace LinealCutOptimizer.Core.Repository
{
    public interface IParamsRepository : IRepository
    {
        Params Get(bool includeDeps = true);
        Params BeginUpdate(Params pParams);
    }
}