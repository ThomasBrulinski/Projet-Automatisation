namespace app.dtos;

public class GetMigrationDto
{
    public IEnumerable<MigrationDto> Migrations { get; set; }
    public int TotalCount { get; set; }
    public int Debut { get; set; }
    public int Fin { get; set; }

    public GetMigrationDto(IEnumerable<MigrationDto> migrations, int totalCount, int debut, int fin)
    {
        Migrations = migrations;
        TotalCount = totalCount;
        Debut = debut;
        Fin = fin;
    }
}