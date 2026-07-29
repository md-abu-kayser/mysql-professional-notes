using System;
using System.Data;
using MySqlConnector;

class Program
{
    static void Main(string[] args)
    {
        string connString = args.Length > 0 ? args[0] : "Server=localhost;Database=test;User ID=root;Password=";

        using var conn = new MySqlConnection(connString);
        conn.Open();

        // Ensure the stored procedure exists (ignore if already exists)
        try
        {
            using var setupCmd = new MySqlCommand(
                @"CREATE PROCEDURE IF NOT EXISTS GetEmployeeCountByPosition(IN pos VARCHAR(50), OUT cnt INT)
                  BEGIN
                    SELECT COUNT(*) INTO cnt FROM employees WHERE position = pos;
                  END", conn);
            setupCmd.ExecuteNonQuery();
            Console.WriteLine("Stored procedure created/verified.");
        }
        catch (MySqlException ex) when (ex.Number == 1304) // Already exists
        {
            Console.WriteLine("Procedure already exists, continuing.");
        }

        // Call the stored procedure
        using var cmd = new MySqlCommand("GetEmployeeCountByPosition", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        // Input parameter
        cmd.Parameters.AddWithValue("@pos", "Developer");

        // Output parameter
        cmd.Parameters.Add(new MySqlParameter("@cnt", MySqlDbType.Int32)
        {
            Direction = ParameterDirection.Output
        });

        // Execute
        cmd.ExecuteNonQuery();

        // Retrieve output parameter value
        int count = Convert.ToInt32(cmd.Parameters["@cnt"].Value);
        Console.WriteLine($"Number of 'Developer' employees: {count}");
    }
}