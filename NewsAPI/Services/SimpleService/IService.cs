using System.Linq.Expressions;

namespace NewsAPI.Services.SimpleService
{
    public interface IService<T> where T : class
    {
        Task<T> GetAsync(int id, bool useDTO = true);
        Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> expression, bool useDTO = true);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        int Delete(int id);
        Task<int> MaxIdAsync(int id);
        Task<int> MinIdAsync(int id);
        bool CheckExists(int id);
    }
}
