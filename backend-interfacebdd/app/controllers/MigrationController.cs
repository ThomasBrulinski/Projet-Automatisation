using app.dtos;
using app.models;
using app.responses;
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
        var data = await migrationService.GetAllMigrationAsync(page, query);
        var dataList = data.Items.ToList();
        var res = new ApiResponse<GetMigrationDto>
            (
                200, 
                "Données récupérées avec succès", 
                new GetMigrationDto(dataList, data.TotalCount, page*20, page*20 + 20)
            );
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

            var response = new ApiResponse<ImportResultDto>
            (
                200, 
                "Traitement terminé",
                new ImportResultDto
                {
                    Inserted = res.Item1,
                    Skipped = res.Item2
                }
            );
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erreur interne : {ex.Message}");
        }
    }
}

