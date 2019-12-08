using System;
using System.Data;
using System.Data.SqlClient;

namespace sqldb_basic
{
    class Program
    {
        private const string ENV_VAR_CONN = "AZ203_SQL_CONNECTIONSTRING";

        private static readonly string ConnectionString = Environment.GetEnvironmentVariable(ENV_VAR_CONN);


        static int Main(string[] args)
        {
            Console.WriteLine("Starting...");

            if (String.IsNullOrEmpty(ConnectionString))
            {
                Console.WriteLine($"Please provide a connection string in the {ENV_VAR_CONN} environment variable");
                return 1;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    // Create a table
                    var sql_create = @"
                        DROP TABLE IF EXISTS demo_pets;
                        CREATE TABLE demo_pets (
                        [id] int IDENTITY(1,1) NOT NULL,
                        [name] varchar(50) NOT NULL,
                        CONSTRAINT PK_PET_ID PRIMARY KEY CLUSTERED (id)
                    )";
                    var command_create = new SqlCommand(sql_create, connection);

                    var rows = command_create.ExecuteNonQuery();
                    Console.WriteLine($"Create table result: {rows}");

                    // Insert some rows
                    var sql_insert = @"
                        INSERT INTO demo_pets (name) VALUES ('Fido');
                        INSERT INTO demo_pets (name) VALUES ('Mittens');
                        INSERT INTO demo_pets (name) VALUES ('Fluffy');
                        INSERT INTO demo_pets (name) VALUES ('Stinko');
                        INSERT INTO demo_pets (name) VALUES ('Gremlin');
                    ";
                    var command_insert = new SqlCommand(sql_insert, connection);
                    rows = command_insert.ExecuteNonQuery();
                    Console.WriteLine($"Create table result: {rows}");

                    // Perform a scalar query
                    var sql_count = "SELECT COUNT(*) FROM demo_pets";
                    var command_count = new SqlCommand(sql_count, connection);
                    var count = command_count.ExecuteScalar();
                    Console.WriteLine($"Count of pets: {count}");

                    // Query the table
                    var sql_query = "SELECT * from demo_pets ORDER BY [name]";
                    var command_query = new SqlCommand(sql_query, connection);
                    using (var reader = command_query.ExecuteReader())
                    {
                        Console.WriteLine("Pets on file:");
                        while (reader.Read())
                        {
                            Console.WriteLine(" - {0} - {1}", reader.GetInt32(0), reader.GetString(1));
                        }
                    }

                    // Drop the table
                    var sql_drop = "DROP TABLE demo_pets";
                    var command_drop = new SqlCommand(sql_drop, connection);
                    rows = command_drop.ExecuteNonQuery();
                    Console.WriteLine($"Drop table result: {rows}");
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"Error: {e.ToString()}");
                return 1;
            }

            return 0;

        }
    }
}
