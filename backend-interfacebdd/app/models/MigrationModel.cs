using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.models;

[Table("Migrations")]
public class MigrationModel
{
    public int Id { get; init; }
    
    [Required]
    [MaxLength(64)]
    public string RowHash { get; init; } = string.Empty;
    
    public DateTime? MigrationStartTime { get; init; }

    [MaxLength(50)]
    public string? SubJobId { get; init; }
    
    [MaxLength(512)]
    public string? Title { get; init; }
        
    [MaxLength(50)]
    public string? Type { get; init; }
        
    [MaxLength(255)]
    public string? SourceId { get; init; }
        
    [MaxLength(512)]
    public string? Source { get; init; }
        
    [MaxLength(255)]
    public string? DestinationId { get; init; }
        
    [MaxLength(512)]
    public string? Destination { get; init; }
        
    [MaxLength(20)]
    public string? Size { get; init; }
        
    [MaxLength(20)]
    public string? Status { get; init; }
        
    [MaxLength(20)]
    public string? MigrationAction { get; init; }
      
    [MaxLength(512)]
    public string? Comment { get; init; }
       
    [MaxLength(255)]
    public string? ErrorCode { get; init; }
}