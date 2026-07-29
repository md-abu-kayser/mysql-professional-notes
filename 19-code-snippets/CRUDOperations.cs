using System;
using MySqlConnector;

class Program
{
    static void Main(string[] args)
    {
        string connString = args.Length > 0 ? args[0] : "Server=localhost;Database=test;User ID=root;Password=";

        using var conn = new MySqlConnection(connString);
        conn.Open();

        // Create table if not exists
        using var setupCmd = new MySqlCommand(
            @"CREATE TABLE IF NOT EXISTS employees (
                id INT AUTO_INCREMENT PRIMARY KEY,
                name VARCHAR(100) NOT NULL,
                position VARCHAR(50),
                salary DECIMAL(10,2)
              )", conn);
        setupCmd.ExecuteNonQuery();
        Console.WriteLine("Table 'employees' ensured.");

        // INSERT - parameterized
        string insertSql = "INSERT INTO employees (name, position, salary) VALUES (@name, @position, @salary)";
        using var insertCmd = new MySqlCommand(insertSql, conn);
        insertCmd.Parameters.AddWithValue("@name", "John Doe");
        insertCmd.Parameters.AddWithValue("@position", "Developer");
        insertCmd.Parameters.AddWithValue("@salary", 75000.00m);
        int rows = insertCmd.ExecuteNonQuery();
        Console.WriteLine($"Inserted {rows} row(s). ID = {insertCmd.LastInsertedId}");

        // SELECT
        string selectSql = "SELECT id, name, position, salary FROM employees WHERE name LIKE @name";
        using var selectCmd = new MySqlCommand(selectSql, conn);
        selectCmd.Parameters.AddWithValue("@name", "%Doe%");
        using var reader = selectCmd.ExecuteReader();
        Console.WriteLine("\nEmployees:");
        while (reader.Read())
            Console.WriteLine($"  ID:{reader.GetInt32(0)} Name:{reader.GetString(1)} Position:{reader.GetString(2)} Salary:{reader.GetDecimal(3):C}");

        reader.Close();  // Need to close before update

        // UPDATE - parameterized
        string updateSql = "UPDATE employees SET salary = @salary WHERE name = @name";
        using var updateCmd = new MySqlCommand(updateSql, conn);
        updateCmd.Parameters.AddWithValue("@salary", 80000.00m);
        updateCmd.Parameters.AddWithValue("@name", "John Doe");
        int updatedRows = updateCmd.ExecuteNonQuery();
        Console.WriteLine($"\nUpdated {updatedRows} row(s).");

        // DELETE - parameterized
        string deleteSql = "DELETE FROM employees WHERE name = @name";
        using var deleteCmd = new MySqlCommand(deleteSql, conn);
        deleteCmd.Parameters.AddWithValue("@name", "John Doe");
        int deletedRows = deleteCmd.ExecuteNonQuery();
        Console.WriteLine($"Deleted {deletedRows} row(s).");
    }
}