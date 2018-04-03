using System;

namespace LinealCutOptimizer.Core.Repository.Implementation
{
    internal class BaseRepository : IDisposable
    {
        private EFLinearCutSolutionEntities _context;

        protected EFLinearCutSolutionEntities DbContext => _context ?? (_context = new EFLinearCutSolutionEntities());

        public void Save()
        {
            DbContext.SaveChanges();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}