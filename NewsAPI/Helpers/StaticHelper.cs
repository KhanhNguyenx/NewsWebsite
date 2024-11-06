using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Data;

namespace NewsAPI.Helpers
{
    public static class StaticHelper
    {
        private static string strcn = "";
        private static IConfiguration myConfig = new ConfigurationBuilder()
                                                        .SetBasePath(Directory.GetCurrentDirectory())
                                                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                                        .Build();
        static StaticHelper()
        {
            strcn = myConfig.GetConnectionString("MyConnect")!;
        }
        public static int ExecuteNonQuery(string sp_name)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcn))
                {
                    SqlCommand cmd = new SqlCommand(sp_name, con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (con.State == ConnectionState.Closed)
                        con.Open();
                    int rowAffected = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    return rowAffected;
                }
            }
            catch { return -1; }
        }

        public static int ExecuteNonQuery(string sp_name, string[] arrParams, object[] arrValues)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(strcn))
                {
                    SqlCommand cmd = new SqlCommand(sp_name, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (arrParams != null)
                    {
                        for (int i = 0; i < arrParams.Length; i++)
                        {
                            cmd.Parameters.AddWithValue(arrParams[i], arrValues[i]);
                        }
                    }
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    int rowAffected = cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    return rowAffected;
                }
            }
            catch
            {
                return -1;
            }
        }

        public static async Task<DbDataReader> ExecuteReaderAsync<T>(string sp_name) where T : new()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(strcn))
                {
                    SqlCommand cmd = new SqlCommand(sp_name, con);
                    cmd.CommandText = sp_name;
                    if (con.State == ConnectionState.Closed)
                        await con.OpenAsync();
                    return await cmd.ExecuteReaderAsync();
                }
            }
            catch { return null!; }
        }

        public static async Task<DbDataReader> ExecuteReaderAsync<T>(string sp_name, string[] arrParams, object[] arrValues) where T : new()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(strcn))
                {
                    SqlCommand cmd = new SqlCommand(sp_name, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (arrParams != null)
                    {
                        for (int i = 0; i < arrParams.Length; i++)
                        {
                            cmd.Parameters.AddWithValue(arrParams[i], arrValues[i]);
                        }
                    }
                    if (conn.State == ConnectionState.Closed)
                        await conn.OpenAsync();
                    return await cmd.ExecuteReaderAsync();
                }
            }
            catch { return null!; }
        }

        public static async Task<List<T>> ConvertToListAsync<T>(this DbDataReader reader) where T : new()
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

        public static async Task<List<T>> GetListAsync<T>(string sp_name) where T : new()
        {
            List<T> list = new List<T>();
            try
            {
                using (SqlConnection conn = new SqlConnection(strcn))
                {
                    SqlCommand cmd = new SqlCommand(sp_name, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (conn.State == ConnectionState.Closed)
                        await cmd.Connection.OpenAsync();

                    var reader = await cmd.ExecuteReaderAsync();
                    list = await reader.ConvertToListAsync<T>();
                    return list;
                }
            }
            catch { return list; }
        }

        public static async Task<List<T>> GetListAsync<T>(string sp_name, string[] arrParams, object[] arrValues) where T : new()
        {
            List<T> list = new List<T>();
            try
            {
                using (SqlConnection conn = new SqlConnection(strcn))
                {
                    SqlCommand cmd = new SqlCommand(sp_name, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (arrParams != null)
                    {
                        for (int i = 0; i < arrParams.Length; i++)
                        {
                            cmd.Parameters.AddWithValue(arrParams[i], arrValues[i]);
                        }
                    }
                    if (conn.State == ConnectionState.Closed)
                        await cmd.Connection.OpenAsync();

                    var reader = await cmd.ExecuteReaderAsync();
                    list = await reader.ConvertToListAsync<T>();
                    return list;
                }
            }
            catch { return list; }
        }

        public static async Task<T> GetSingleAsync<T>(string sp_name, string[] arrParams, object[] arrValues) where T : new()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(strcn))
                {
                    SqlCommand cmd = new SqlCommand(sp_name, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (arrParams != null)
                    {
                        for (int i = 0; i < arrParams.Length; i++)
                        {
                            cmd.Parameters.AddWithValue(arrParams[i], arrValues[i]);
                        }
                    }
                    if (conn.State == ConnectionState.Closed)
                        await cmd.Connection.OpenAsync();

                    var reader = await cmd.ExecuteReaderAsync();
                    var list = await reader.ConvertToListAsync<T>();
                    return list.SingleOrDefault()!;
                }
            }
            catch { return new T(); }
        }
    }
}
