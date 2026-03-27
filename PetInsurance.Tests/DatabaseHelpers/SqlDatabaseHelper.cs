using Dapper;
using Microsoft.Data.Sqlite;
using PetInsurance.Tests.Models;
using System.Data;

namespace PetInsurance.Tests.DatabaseHelpers
{
    public class SqlDatabaseHelper : IDisposable
    {
        private readonly IDbConnection _connection;
        private bool _disposed;

        // 🔍 Конструктор для тестів: створює БД в пам'яті
        public SqlDatabaseHelper()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            CreateSchema();
        }

        // 🔍 Конструктор для реальних тестів з конфігурації
        public SqlDatabaseHelper(string connectionString)
        {
            _connection = new SqliteConnection(connectionString);
            _connection.Open();
            CreateSchema();
        }

        private void CreateSchema()
        {
            var createTableSql = @"
                CREATE TABLE IF NOT EXISTS Claims (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PetName TEXT NOT NULL,
                    OwnerName TEXT NOT NULL,
                    Amount REAL NOT NULL,
                    Status TEXT DEFAULT 'pending',
                    CreatedAt TEXT NOT NULL,
                    ProcessedAt TEXT
                );
                
                CREATE INDEX IF NOT EXISTS IX_Claims_Status ON Claims(Status);
                CREATE INDEX IF NOT EXISTS IX_Claims_CreatedAt ON Claims(CreatedAt);
            ";
            _connection.Execute(createTableSql);
        }

        // 🔍 CRUD-операції для тестів
        public async Task<int> CreateClaimAsync(Claim claim)
        {
            var sql = @"
                INSERT INTO Claims (PetName, OwnerName, Amount, Status, CreatedAt, ProcessedAt)
                VALUES (@PetName, @OwnerName, @Amount, @Status, @CreatedAt, @ProcessedAt);
                SELECT last_insert_rowid();";
            
            return await _connection.ExecuteScalarAsync<int>(sql, claim);
        }

        public async Task<Claim?> GetClaimByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<Claim>(
                "SELECT * FROM Claims WHERE Id = @Id", new { Id = id });
        }

        public async Task<List<Claim>> GetClaimsByStatusAsync(string status)
        {
            return (await _connection.QueryAsync<Claim>(
                "SELECT * FROM Claims WHERE Status = @Status", new { Status = status })).ToList();
        }

        public async Task<List<Claim>> GetClaimsByOwnerNameAsync(string ownerName)
        {
            var sql = "SELECT * FROM Claims WHERE OwnerName = @OwnerName";
            var claims = await _connection.QueryAsync<Claim>(sql, new { OwnerName = ownerName });
            return claims.ToList();
        }

        public async Task<bool> UpdateClaimStatusAsync(int id, string newStatus)
        {
            var sql = @"
                UPDATE Claims 
                SET Status = @Status, ProcessedAt = @ProcessedAt 
                WHERE Id = @Id";
            
            var result = await _connection.ExecuteAsync(sql, new 
            { 
                Id = id, 
                Status = newStatus,
                ProcessedAt = newStatus != "pending" ? DateTime.UtcNow : (DateTime?)null
            });
            
            return result > 0;
        }

        public async Task<bool> DeleteClaimAsync(int id)
        {
            var result = await _connection.ExecuteAsync(
                "DELETE FROM Claims WHERE Id = @Id", new { Id = id });
            return result > 0;
        }

        // 🔍 Метод для очищення БД між тестами
        public async Task ClearAllClaimsAsync()
        {
            await _connection.ExecuteAsync("DELETE FROM Claims");
        }

        // 🔍 IDisposable для правильного закриття з'єднання
        public void Dispose()
        {
            if (!_disposed)
            {
                _connection?.Close();
                _connection?.Dispose();
                _disposed = true;
            }
        }
    }
}