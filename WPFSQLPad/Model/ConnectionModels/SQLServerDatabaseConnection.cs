using System.Data.SqlClient;

namespace Model.ConnectionModels
{
    public sealed class SQLServerDatabaseConnection : DatabaseConnection
    {

        public SQLServerDatabaseConnection(string server, string database, string userId, string password)
        : base(server, database, userId, password, DbType.SQLServer)
        {
            Description = $"{server}: {database} (SQL Server)";

            CreateConnection();

            connectionCheck = new ExternalTimeDispatcher(this);
        }
        
        protected override void CreateConnection()
        {
            string connectionString =
                $"Data Source={Server};Initial Catalog={Database};User ID={UserId};Password={password}";
            
            connection = new SqlConnection(connectionString);

            IsAvailable = OpenConnection();
        }

        public override bool Ping()
        {
            //todo
            return true;
        }
        
    }
}
