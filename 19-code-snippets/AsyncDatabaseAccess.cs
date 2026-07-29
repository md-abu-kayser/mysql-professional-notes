using System;
using System.Threading.Tasks;
using MySqlConnector;

class Program
{
    static async Task Main(string[] args)
    {
        string connString = args.Length > 0 ? args[0] : "Server=localhost;Database=mysql;User ID=root;Password=";

        await using var conn = new MySqlConnection(connString);
        await conn.OpenAsync();
        Console.WriteLine("Connected asynchronously.");

        // Execute two independent queries concurrently
        Task<long> tableCountTask = GetTableCountAsync(conn);
        Task<string> versionTask = GetServerVersionAsync(conn);

        await Task.WhenAll(tableCountTask, versionTask);

        Console.WriteLine($"Server version: {versionTask.Result}");
        Console.WriteLine($"Number of tables: {tableCountTask.Result}");
    }

    static async Task<long> GetTableCountAsync(MySqlConnection conn)
    {
        await using var cmd = new MySqlCommand("SELECT COUNT(*) FROM information_schema.tables", conn);
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt64(result);
    }

    static async Task<string> GetServerVersionAsync(MySqlConnection conn)
    {
        await using var cmd = new MySqlCommand("SELECT VERSION()", conn);
        var result = await cmd.ExecuteScalarAsync();
        return result.ToString();
    }
}