namespace LinealCutOptimizer.Core.Repository.Implementation
{
    internal class BaseRepository
    {
        public void Save()
        {
            using (var context = new EFLinearCutSolutionEntities())
            {
                context.SaveChanges();
            }
        }
    }
}
