using Microsoft.EntityFrameworkCore;
using NewsAPI.Data;
using NewsAPI.Models;

namespace NewsAPI.Services
{
    public interface ILogService
    {
        Task<List<Log>> GetListAsync();
    }
    public class LogService : ILogService
    {
        private readonly NewsWebDbContext _context;
        public LogService(NewsWebDbContext context)
        {
            _context = context;
        }
        public async Task<List<Log>> GetListAsync()
        {
            return await _context.Logs.ToListAsync();
        }
    }
}
