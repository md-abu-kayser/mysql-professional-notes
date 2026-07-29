using System;
using MySqlConnector;

class Program
{
    static void Main(string[] args)
    {
        string connString = args.Length > 0 ? args[0] : "Server=localhost;Database=test;User ID=root;Password=";

        using var conn = new MySqlConnection(connString);
        conn.Open();

        // Setup: create and seed accounts table
        using (var cmd = new MySqlCommand(
            @"CREATE TEMPORARY TABLE IF NOT EXISTS accounts (
                id INT PRIMARY KEY,
                balance DECIMAL(10,2)
              );
              INSERT INTO accounts VALUES (1, 1000), (2, 500)
              ON DUPLICATE KEY UPDATE balance=VALUES(balance);", conn))
        {
            cmd.ExecuteNonQuery();
        }

        // Successful transaction
        using (var tx = conn.BeginTransaction())
        {
            try
            {
                using var deduct = new MySqlCommand("UPDATE accounts SET balance = balance - 200 WHERE id = 1", conn, tx);
                deduct.ExecuteNonQuery();

                using var add = new MySqlCommand("UPDATE accounts SET balance = balance + 200 WHERE id = 2", conn, tx);
                add.ExecuteNonQuery();

                tx.Commit();
                Console.WriteLine("Transfer of 200 completed.");
            }
            catch (Exception)
            {
                tx.Rollback();
                Console.Error.WriteLine("Transaction rolled back.");
                throw;
            }
        }

        // Failed transaction (rollback due to missing account)
        using (var tx = conn.BeginTransaction())
        {
            try
            {
                using var deduct = new MySqlCommand("UPDATE accounts SET balance = balance - 300 WHERE id = 1", conn, tx);
                deduct.ExecuteNonQuery();

                // This will fail (id=3 does not exist)
                using var add = new MySqlCommand("UPDATE accounts SET balance = balance + 300 WHERE id = 3", conn, tx);
                add.ExecuteNonQuery();

                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                Console.WriteLine($"Rolled back because: {ex.Message}");
            }
        }

        // Show final balances
        using var show = new MySqlCommand("SELECT id, balance FROM accounts ORDER BY id", conn);
        using var reader = show.ExecuteReader();
        Console.WriteLine("\nFinal balances:");
        while (reader.Read())
            Console.WriteLine($"  Account {reader.GetInt32(0)}: {reader.GetDecimal(1):C}");
    }
}