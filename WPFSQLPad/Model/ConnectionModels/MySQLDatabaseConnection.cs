using MySql.Data.MySqlClient;

namespace Model.ConnectionModels
{

    /// <summary>
    /// Handler for database connection.
    /// </summary>
    public sealed class MySQLDatabaseConnection : DatabaseConnection
    {

        public MySQLDatabaseConnection(string server, string database, string userId, string password)
        : base(server, database, userId, password, DbType.MySQL)
        {
            Description = $"{server}: {database} (MySQL)";

            CreateConnection();

            connectionCheck = new ExternalTimeDispatcher(this);
        }

        protected override void CreateConnection()
        {
            string connectionString = $"server={Server};database={Database};uid={UserId};pwd={password};sslmode=none";
            connection = new MySqlConnection(connectionString);

            IsAvailable = OpenConnection();
        }

        public override bool Ping()
        {
            return ((MySqlConnection) connection).Ping();
        }

    }
}
