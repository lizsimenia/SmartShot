using MySqlConnector;
//using System;
//using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.IO;

namespace AppCurs
{
    internal class DatabaseManager
    {
        private readonly MySqlConnection _connection;
        private bool _disposed = false;
        private bool _isConnected = false;


        // Конструктор: открывает соединение с БД
        public DatabaseManager(string connectionString)
        { 
             try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string mysqlBin = Path.Combine(baseDir, "mysql-5.7", "bin", "mysqld.exe");
                
                if (File.Exists(mysqlBin))
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = mysqlBin,
                        UseShellExecute = false,
                        CreateNoWindow = true,   
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                    Process.Start(psi);
                }
                _connection = new MySqlConnection(connectionString);
                _connection.Open();
                _isConnected = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database connection failed: {ex.Message}");
                _isConnected = false;
                _connection = null;
            }
        }


        // Запись в таблицу activity
        public int LogActivity(int eventType, int status)
        {
            if (_isConnected)
            {
                const string sql = @"
                INSERT INTO activity (TypeOp, StatusOp, DateTimeOp)
                VALUES (@eventType, @status, NOW());
                SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(sql, _connection))
                {
                    cmd.Parameters.AddWithValue("@eventType", eventType);
                    cmd.Parameters.AddWithValue("@status", status);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return -1;
        }

        // Запись в таблицу processing
        public void LogProcessing(int activityId, DateTime startTime, DateTime endTime, string config, int photoCount)
        {
            if (_isConnected)
            {
                double processingTimeMs = (endTime - startTime).TotalMilliseconds;
                const string sql = @"
                INSERT INTO processing (IDop, DateTimeStartProcessing, DateTimeEndProcessing, ConfigProcessing, TimeProcessing, CountPhotos)
                VALUES (@id, @start, @end, @config, @time, @count);";

                using (var cmd = new MySqlCommand(sql, _connection))
                {
                    cmd.Parameters.AddWithValue("@id", activityId);
                    cmd.Parameters.AddWithValue("@start", startTime);
                    cmd.Parameters.AddWithValue("@end", endTime);
                    cmd.Parameters.AddWithValue("@config", config);
                    cmd.Parameters.AddWithValue("@time", processingTimeMs);
                    cmd.Parameters.AddWithValue("@count", photoCount);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Закрытие соединения при освобождении ресурсов
        public void Dispose()
        {
            if (!_disposed)
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();
                _connection.Dispose();
                _disposed = true;
            }
        }
    }
}
