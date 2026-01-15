using app.models;
using app.services;
using Microsoft.AspNetCore.Mvc;

namespace app.controllers;

[ApiController]
[Route("api/migration/")]
public class MigrationController(MigrationService migrationService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAllRow([FromQuery] int page = 0, [FromQuery] string query = "")
    {
        var res = await migrationService.GetAllMigrationAsync(page, query);
        return Ok(res);
    }
    
    [HttpPost]
    public async Task<IActionResult> UploadRaws([FromBody] List<MigrationModel>? rows)
    {
        // 1. Vérifie si le corps de la requête est vide ou n'est pas une liste
        if (rows == null || rows.Count == 0)
        {
            return BadRequest("Le corps de la requête est vide ou n'est pas une liste de données valide.");
        }

        // 2. Vérification de la validité du modèle (Annotations [Required], [MaxLength]...)
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Retourne les erreurs de validation précises
        }

        try
        {
            (int, int) res = await migrationService.ProcessBatchAsync(rows);

            return Ok(new { 
                message = "Données reçues et validées", 
                inserted = res.Item1, 
                skipped = res.Item2,
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erreur interne : {ex.Message}");
        }
    }
}

