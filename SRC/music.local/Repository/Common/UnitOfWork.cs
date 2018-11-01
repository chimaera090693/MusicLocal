using System;
using System.Data.Entity;

namespace music.local.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        DbSet<T> Set<T>() where T : class;
        int Commit();
    }

    public class UnitOfWork : IUnitOfWork
    {
        readonly AppDbContext _context;
        private bool _isDisposed;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public DbSet<T> Set<T>() where T : class
        {
            return _context.Set<T>();
        }

        public int Commit()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            _context.Dispose();
        }
    }
}