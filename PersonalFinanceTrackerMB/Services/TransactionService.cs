using Microsoft.Data.Sqlite;
using PersonalFinanceTrackerMB.Models;

namespace PersonalFinanceTrackerMB.Services;

public class TransactionService
{
    private readonly string dbPath;

    public TransactionService(string databasePath)
    {
        dbPath = databasePath;
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Transactions (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Amount REAL NOT NULL,
                Category TEXT NOT NULL,
                Date TEXT NOT NULL,
                Type TEXT NOT NULL
            );";
        command.ExecuteNonQuery();
    }

    public async Task AddTransactionAsync(Transaction transaction)
    {
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Transactions (Amount, Category, Date, Type) 
            VALUES ($amount, $category, $date, $type)";
        command.Parameters.AddWithValue("$amount", transaction.Amount);
        command.Parameters.AddWithValue("$category", transaction.Category);
        command.Parameters.AddWithValue("$date", transaction.Date.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$type", transaction.Type);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<Transaction>> GetTransactionsAsync()
    {
        var transactions = new List<Transaction>();
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Transactions";
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            transactions.Add(new Transaction
            {
                Id = reader.GetInt32(0),
                Amount = reader.GetDecimal(1),
                Category = reader.GetString(2),
                Date = DateTime.Parse(reader.GetString(3)),
                Type = reader.GetString(4)
            });
        }
        return transactions;
    }

    // New method to get total income asynchronously
    public async Task<decimal> GetTotalIncomeAsync()
    {
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT SUM(Amount) FROM Transactions WHERE Type = 'Income'";

        var result = await command.ExecuteScalarAsync();
        return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
    }

    // New method to get total expenses asynchronously
    public async Task<decimal> GetTotalExpensesAsync()
    {
        using var connection = new SqliteConnection($"Data Source={dbPath}");
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT SUM(Amount) FROM Transactions WHERE Type = 'Expense'";

        var result = await command.ExecuteScalarAsync();
        return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
    }
}
