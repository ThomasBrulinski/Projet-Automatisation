namespace app.dtos;

public record MigrationDto
{
    public DateTime? MigrationStartTime { get; init; }
    public string? SubJobId { get; init; }
    public string? Title { get; init; }
    public string? Type { get; init; }
    public string? SourceId { get; init; }
    public string? Source { get; init; }
    public string? DestinationId { get; init; }
    public string? Destination { get; init; }
    public string? Status { get; init; }
    public string? Size { get; init; }
    public string? ErrorCode { get; init; }
    public string? Comment { get; init; }
}