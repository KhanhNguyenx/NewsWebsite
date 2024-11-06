using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NewsAPI.Data;
using NewsAPI.Models.APIHelperModels;
using System.Data.Common;
using System.Data;
using NewsAPI.Helpers;

namespace NewsAPI.Services
{
    public interface ISqlHelperService
    {
        Task<DataResponse> ExecuteReader<T>(string sp_name) where T : new();
        Task<DataResponse> ExecuteReader<T>(string sp_name, string[] arrParams, object[] arrValues) where T : new();
        Task<DataResponse> Login<T>(string sp_name, string[] arrParams, object[] arrValues) where T : new();
    }
    public class SqlHelperService : ISqlHelperService
    {
        private readonly NewsWebDbContext _context;
        private readonly IConfiguration _configuration;
        public SqlHelperService(NewsWebDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<DataResponse> ExecuteReader<T>(string sp_name) where T : new()
        {
            var res = new DataResponse();
            try
            {
                var cmd = _context.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = sp_name;
                cmd.CommandType = CommandType.StoredProcedure;
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                var reader = await cmd.ExecuteReaderAsync();
                if (reader != null && reader.HasRows)
                {
                    var list = await this.ToListAsync<T>(reader);
                    if (list.Count > 0)
                    {
                        res.Data = list;
                        res.Success = true;
                        res.Message = "Ok";
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                res.Message = $"Exception: Xẩy ra lỗi khi đọc dữ liệu {ex.Message}";
                return res;
            }
        }

        public async Task<DataResponse> ExecuteReader<T>(string sp_name, string[] arrParams, object[] arrValues) where T : new()
        {
            var res = new DataResponse();
            try
            {
                var cmd = _context.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = sp_name;
                cmd.CommandType = CommandType.StoredProcedure;
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                if (arrParams != null)
                {
                    for (int i = 0; i < arrParams.Length; i++)
                    {
                        cmd.Parameters.Add(new SqlParameter(arrParams[i], arrValues[i]));
                    }
                }
                var reader = await cmd.ExecuteReaderAsync();
                if (reader != null && reader.HasRows)
                {
                    var list = await this.ToListAsync<T>(reader);
                    if (list.Count > 0)
                    {
                        res.Data = list;
                        res.Success = true;
                        res.Message = "Ok";
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                res.Message = $"Exception: Xẩy ra lỗi khi đọc dữ liệu {ex.Message}";
                return res;
            }
        }

        public async Task<DataResponse> Login<T>(string sp_name, string[] arrParams, object[] arrValues) where T : new()
        {
            var res = new DataResponse();
            try
            {
                var cmd = _context.Database.GetDbConnection().CreateCommand();
                cmd.CommandText = sp_name;
                cmd.CommandType = CommandType.StoredProcedure;
                if (cmd.Connection.State != ConnectionState.Open)
                    cmd.Connection.Open();
                if (arrParams != null)
                {
                    for (int i = 0; i < arrParams.Length; i++)
                    {
                        cmd.Parameters.Add(new SqlParameter(arrParams[i], arrValues[i]));
                    }
                }
                var reader = await cmd.ExecuteReaderAsync();
                var list = await reader.ConvertToListAsync<T>();
                if (list != null && list.Count > 0)
                {
                    res.Data = list.SingleOrDefault()!;
                    res.Success = true;
                    res.Message = "Ok";
                }
                return res;
            }
            catch { return null!; }
        }

        private async Task<List<T>> ToListAsync<T>(DbDataReader reader) where T : new()
        {
            List<T> list = new List<T>();
            if (reader != null && reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    T obj = new T();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        // Lấy tên cột và giá trị của cột
                        string columnName = reader.GetName(i);
                        object columnValue = reader.GetValue(i);
                        // Tìm thuộc tính tương ứng trong đối tượng T và thiết lập giá trị của nó
                        var property = typeof(T).GetProperty(columnName);
                        if (property != null && columnValue != DBNull.Value)
                        {
                            property.SetValue(obj, columnValue);
                        }
                    }
                    list.Add(obj);
                }
            }
            return list;
        }
    }
}
