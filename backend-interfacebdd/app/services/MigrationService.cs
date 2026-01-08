using app.dtos;
using app.models;
using app.repositories.interfaces;

namespace app.services;

public class MigrationService(IMigrationRepository migrationRepository)
{
    public async Task<(int inserted, int skipped)> ProcessBatchAsync(List<MigrationModel> rows)
    {
        return await migrationRepository.ProcessBatchAsync(rows);
    }

    public async Task<IEnumerable<MigrationDto>> GetAllMigrationAsync(int page = 0, string query = "")
    {
        var models =  await migrationRepository.GetAllRowAsync(page, query);
        return models.Select(m => new MigrationDto
        {
            MigrationStartTime = m.MigrationStartTime,
            SubJobId = m.SubJobId,
            Title = m.Title,
            Type = m.Type,
            SourceId = m.SourceId,
            Source = m.Source,
            DestinationId = m.DestinationId,
            Destination = m.Destination,
            Status = m.Status,
            Size = m.Size,
            ErrorCode = m.ErrorCode,
            Comment = m.Comment
        });
    }
}