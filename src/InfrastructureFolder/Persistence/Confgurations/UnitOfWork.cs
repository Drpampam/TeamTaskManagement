using Application.Interfaces;

namespace Persistence.Confgurations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
           
        }

        public int CommitChanges()
        {
            return _context.SaveChanges();
        }

        public Task<int> CommitChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
