using System;
using MySqlConnector;

class Program
{
    static void Main(string[] args)
    {
        string connString = args.Length > 0
            ? args[0]
            : "Server=localhost;Database=mysql;User ID=root;Password=";

        try
        {
            using var conn = new MySqlConnection(connString);
            conn.Open();
            Console.WriteLine($"Connected to MySQL server version: {conn.ServerVersion}");

            string sql = "SELECT table_schema, table_name, table_rows FROM information_schema.tables WHERE table_schema NOT IN ('mysql','information_schema','performance_schema','sys') LIMIT 10";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            // Print headers
            Console.WriteLine($"{"Schema",-20} {"Table",-30} {"Rows",10}");
            Console.WriteLine(new string('-', 60));

            while (reader.Read())
            {
                string schema = reader.GetString(0);
                string table  = reader.GetString(1);
                long rows     = reader.IsDBNull(2) ? 0 : reader.GetInt64(2);
                Console.WriteLine($"{schema,-20} {table,-30} {rows,10}");
            }
        }
        catch (MySqlException ex)
        {
            Console.Error.WriteLine($"MySQL Error: {ex.Number} - {ex.Message}");
            Environment.Exit(1);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"General Error: {ex.Message}");
            Environment.Exit(1);
        }
    }
}