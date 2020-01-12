# Azure SQL DB

* MSFT Docs: https://docs.microsoft.com/en-us/azure/sql-database/
* Demo: <SqlServer/README.md>


## Connection String

    Server=tcp:az203-apidbserver.database.windows.net,1433;Initial Catalog=az203-todo;Persist Security Info=False;User ID=dba;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;

For local development:

    Server=(localdb)\\mssqllocaldb;Database=TodoContext;Trusted_Connection=True;MultipleActiveResultSets=true

## Example code

    using (SqlConnection connection = new SqlConnection(ConnectionString))
    {
        connection.Open();

        // Insert rows
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
    }
