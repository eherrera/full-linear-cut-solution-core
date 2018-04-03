namespace LinealCutOptimizer.Core.Repository
{
    public interface IParamsRepository : IRepository
    {
        Params Get();
        Params BeginUpdate(Params pParams);
    }
}