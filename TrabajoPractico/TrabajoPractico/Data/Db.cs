using Microsoft.Data.SqlClient;
namespace TrabajoPractico.Data
{
    public class Db
    {
        private readonly string _connStr;
        public Db(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection")
                ?? "Server=localhost\\SQLEXPRESS;Database=ClubesDB;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        public SqlConnection GetConnection() => new SqlConnection(_connStr);
    }
}
