using Microsoft.EntityFrameworkCore;
using NewsAPI.Data;
using System.Linq.Expressions;

namespace NewsAPI.Services
{
    public interface IGenericServive<T> where T : class
    {
        Task<T> GetASync(int id);
        Task<IEnumerable<T>> GetListASync();
        Task<IEnumerable<T>> GetListASync(Expression<Func<T, bool>> exception);
        Task<T> CreateASync(T entity);
        Task<T> UpdateASync(T entity);
        Task<int> DeleteASync(T entity);
        Task<int> MaxIdAsync(Expression<Func<T, int>> exception);
        Task<int> MinIdAsync(Expression<Func<T, int>> exception);
           // Lấy top các đối tượng theo thứ tự ưu tiên
    Task<IEnumerable<T>> GetTopAsync<TKey>(int count, Expression<Func<T, TKey>> orderBy);

    // Lấy các đối tượng mới nhất
    Task<IEnumerable<T>> GetLatestAsync<TKey>(int count, Expression<Func<T, TKey>> orderByDescending);

    }
    public class GenericService<T> : IGenericServive<T> where T : class
    {
        private readonly NewsWebDbContext _context;
        protected readonly DbSet<T> _dbSet;
        public GenericService(NewsWebDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> CreateASync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<int> DeleteASync(T entity)
        {
            try
            {
                _dbSet.Remove(entity);
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<T> GetASync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetListASync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetListASync(Expression<Func<T, bool>> exception)
        {
            return await _dbSet.Where(exception).ToListAsync();
        }

        public async Task<int> MaxIdAsync(Expression<Func<T, int>> exception)
        {
            return await _dbSet.MaxAsync(exception);
        }

        public async Task<int> MinIdAsync(Expression<Func<T, int>> exception)
        {
            return await _dbSet.MinAsync(exception);
        }

        public async Task<T> UpdateASync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
        // Lấy danh sách với số lượng giới hạn và tiêu chí sắp xếp
        public async Task<IEnumerable<T>> GetTopAsync<TKey>(int count, Expression<Func<T, TKey>> orderBy)
        {
            return await _dbSet.OrderByDescending(orderBy).Take(count).ToListAsync();
        }

        // Lấy danh sách mới nhất dựa theo tiêu chí (ngày tạo, thời gian, ...)
        public async Task<IEnumerable<T>> GetLatestAsync<TKey>(int count, Expression<Func<T, TKey>> orderByDescending)
        {
            return await _dbSet.OrderByDescending(orderByDescending).Take(count).ToListAsync();
        }
    }
}
