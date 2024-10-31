using Microsoft.EntityFrameworkCore;
using NewsAPI.Data;
using System.Linq.Expressions;

namespace NewsAPI.Services
{
    public interface IGenericServive<T> where T : class
    {
        Task<T> GetAsync(int id);
        Task<IEnumerable<T>> GetListAsync();
        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> exception);
        Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> expression);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<int> DeleteAsync(T entity);
        int Delete(T entity);
        Task<int> MaxIdAsync(Expression<Func<T, int>> exception);
        Task<int> MinIdAsync(Expression<Func<T, int>> exception);
        // Lấy top các đối tượng theo thứ tự ưu tiên
        Task<IEnumerable<T>> GetTopAsync<TKey>(int count, Expression<Func<T, TKey>> orderBy);
        // Lấy các đối tượng mới nhất
        Task<IEnumerable<T>> GetLatestAsync<TKey>(int count, Expression<Func<T, TKey>> orderByDescending);
        Task<(IEnumerable<T>, int)> GetPagedListAsync(int pageNumber, int pageSize);

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

        public async Task<T> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<int> DeleteAsync(T entity)
        {
            try
            {
                // Giả sử thuộc tính "Status" là để kiểm tra trạng thái của đối tượng (1 = active, 0 = deleted)
                var propertyInfo = entity.GetType().GetProperty("Status");
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(entity, 0); // Đặt Status = 0 (tương đương với việc "xóa")
                }

                _context.Entry(entity).State = EntityState.Modified; // Cập nhật trạng thái
                await _context.SaveChangesAsync(); // Lưu thay đổi
                return 1; // Trả về thành công
            }
            catch
            {
                return 0; // Trả về thất bại
            }
        }
        public int Delete(T entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Deleted;
                var result = _context.SaveChanges();
                return result;
            }
            catch
            {
                return 0;
            }
        }
        public async Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.Where(expression).ToListAsync();
        }
        public async Task<T> GetAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetListAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> exception)
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

        public async Task<T> UpdateAsync(T entity)
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
        public async Task<(IEnumerable<T>, int)> GetPagedListAsync(int pageNumber, int pageSize)
        {
            var totalRecords = await _dbSet.CountAsync();
            var data = await _dbSet
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (data, totalRecords);
        }
    }
}
