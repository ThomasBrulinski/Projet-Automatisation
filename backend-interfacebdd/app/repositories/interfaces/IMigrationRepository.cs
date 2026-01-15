using app.models;

namespace app.repositories.interfaces;

public interface IMigrationRepository
{
    Task<(IEnumerable<MigrationModel> Items, int TotalCount)> GetAllRowAsync(int page = 0, string query = "");
    Task<(int inserted, int skipped)> ProcessBatchAsync(List<MigrationModel> rows);
}