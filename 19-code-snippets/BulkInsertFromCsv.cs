using System;
using System.Data;
using System.IO;
using MySqlConnector;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Usage: BulkInsertFromCsv <connectionString> <csvFilePath> [batchSize]");
            Environment.Exit(1);
        }

        string connString = args[0];
        string csvFilePath = args[1];
        int batchSize = args.Length > 2 ? int.Parse(args[2]) : 1000;

        if (!File.Exists(csvFilePath))
        {
            Console.Error.WriteLine($"CSV file not found: {csvFilePath}");
            Environment.Exit(1);
        }

        using var conn = new MySqlConnection(connString);
        conn.Open();

        // Create target table if not exists (adjust schema as needed)
        using var setupCmd = new MySqlCommand(
            @"CREATE TABLE IF NOT EXISTS employees_bulk (
                id INT AUTO_INCREMENT PRIMARY KEY,
                name VARCHAR(100) NOT NULL,
                position VARCHAR(50),
                salary DECIMAL(10,2)
              )", conn);
        setupCmd.ExecuteNonQuery();

        // Prepare INSERT statement (parameterized)
        string insertSql = "INSERT INTO employees_bulk (name, position, salary) VALUES (@name, @position, @salary)";

        int totalRows = 0;
        var lines = File.ReadLines(csvFilePath);
        bool isFirstLine = true;

        using var transaction = conn.BeginTransaction();
        using var cmd = new MySqlCommand(insertSql, conn, transaction);
        cmd.Parameters.Add("@name", MySqlDbType.VarChar);
        cmd.Parameters.Add("@position", MySqlDbType.VarChar);
        cmd.Parameters.Add("@salary", MySqlDbType.Decimal);

        int currentBatch = 0;

        foreach (var line in lines)
        {
            if (isFirstLine)
            {
                // Skip header line (optional – assumes CSV has headers)
                isFirstLine = false;
                continue;
            }

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var cols = line.Split(',');

            // Very basic CSV parsing – for production use a proper CSV parser (e.g., CsvHelper)
            if (cols.Length < 3) continue;

            cmd.Parameters["@name"].Value = cols[0].Trim();
            cmd.Parameters["@position"].Value = cols[1].Trim();
            cmd.Parameters["@salary"].Value = decimal.Parse(cols[2].Trim());

            cmd.ExecuteNonQuery();
            currentBatch++;
            totalRows++;

            if (currentBatch >= batchSize)
            {
                transaction.Commit();
                Console.WriteLine($"Batch of {currentBatch} rows committed. Total rows: {totalRows}");
                currentBatch = 0;

                // Start new transaction for next batch
                transaction.Dispose();
                transaction = conn.BeginTransaction();
                cmd.Transaction = transaction;
            }
        }

        // Commit any remaining rows
        if (currentBatch > 0)
        {
            transaction.Commit();
            Console.WriteLine($"Final batch of {currentBatch} rows committed. Total rows inserted: {totalRows}");
        }
        else
        {
            transaction.Dispose(); // Nothing to commit
        }

        Console.WriteLine($"\nBulk insert completed. {totalRows} rows inserted.");
    }
}