using app.data;
using app.models;
using app.repositories.interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.repositories.classes;

public class MigrationRepository(AppDbContext context) : IMigrationRepository
{
    public async Task<(IEnumerable<MigrationModel> Items, int TotalCount)> GetAllRowAsync(int page = 0, string query = "")
    {
        const int pageSize = 20;

        // 1. On prépare la requête de base avec le filtre
        var baseQuery = context.MigrationModels
            .Where(m => m.Source.Contains(query));

        // 2. On compte le total global correspondant à la recherche (sans pagination)
        int totalCount = await baseQuery.CountAsync();

        // 3. On récupère uniquement les 20 lignes de la page demandée
        var items = await baseQuery
            .OrderByDescending(m => m.MigrationStartTime) 
            .Skip(page * pageSize)
            .Take(pageSize)         
            .ToListAsync();

        // 4. On renvoie les deux informations
        return (items, totalCount);
    }
    
    // On change le type de retour en Task<(int inserted, int skipped)>
    public async Task<(int inserted, int skipped)> ProcessBatchAsync(List<MigrationModel> rows)
    {
        // 1. On récupère les hashs entrants
        var incomingHashes = rows.Select(r => r.RowHash).ToList();

        // 2. On récupère les hashs qui existent déjà en BDD
        var existingHashes = await context.MigrationModels
            .Where(m => incomingHashes.Contains(m.RowHash))
            .Select(m => m.RowHash)
            .ToListAsync();

        // 3. Calcul des statistiques
        var skippedCount = existingHashes.Count;
        var newRows = rows.Where(r => !existingHashes.Contains(r.RowHash)).ToList();
        var insertedCount = newRows.Count;

        // 4. On insère uniquement les nouveautés
        if (insertedCount <= 0) return (inserted: insertedCount, skipped: skippedCount);
        
        await context.MigrationModels.AddRangeAsync(newRows);
        await context.SaveChangesAsync();

        // On renvoie un tuple avec les deux valeurs
        return (inserted: insertedCount, skipped: skippedCount);
    }
}