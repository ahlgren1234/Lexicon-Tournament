using Microsoft.AspNetCore.Mvc;
using Service.Contracts.DTO;
using Service.Contracts.Services;
using Microsoft.AspNetCore.JsonPatch;

namespace Tournament.Api.Controllers;

[Route("api/TournamentDetails")]
[ApiController]
public class TournamentDetailsController : ControllerBase
{
    private readonly ITournamentService _tournamentService;

    public TournamentDetailsController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }


    // GET: api/TournamentDetails
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TournamentDTO>>> GetTournamentDetails(
        [FromQuery] bool includeGames = false,
        [FromQuery] string? title = null,
        [FromQuery] string? sortBy = null)
    {
        var tournaments = await _tournamentService.GetAllAsync(includeGames);

        if (!string.IsNullOrWhiteSpace(title))
        {
            tournaments = tournaments
                .Where(t => t.Title != null && t.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        tournaments = sortBy?.ToLower() switch
        {
            "title" => tournaments.OrderBy(t => t.Title).ToList(),
            "startdate" => tournaments.OrderBy(t => t.StartDate).ToList(),
            _ => tournaments
        };

        return Ok(tournaments);
    }

    // GET: api/TournamentDetails/5 
    [HttpGet("{id}")]
    public async Task<ActionResult<TournamentDTO>> GetTournamentById(int id)
    {
        var tournament = await _tournamentService.GetAsync(id);

        if (tournament == null)
        {
            return NotFound();
        }

        return Ok(tournament);
    }

    // PUT: api/TournamentDetails/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTournamentDetails(int id, TournamentDTO tournamentDTO)
    {
        if (id != tournamentDTO.Id)
        {
            return BadRequest();
        }

        var result = await _tournamentService.UpdateAsync(id, tournamentDTO);
        
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    // POST: api/TournamentDetails
    [HttpPost]
    public async Task<ActionResult<TournamentDTO>> PostTournamentDetails(TournamentDTO tournamentDTO)
    {
        var createdTournament = await _tournamentService.AddAsync(tournamentDTO);
        return CreatedAtAction(nameof(GetTournamentById), new { id = createdTournament.Id }, createdTournament);
    }

    // DELETE: api/TournamentDetails/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTournamentDetails(int id)
    {
        var result = await _tournamentService.DeleteAsync(id);
        
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<TournamentDTO>> PatchTournament(int id, JsonPatchDocument<TournamentDTO> patchDocument)
    {
        if (patchDocument == null)            
            return BadRequest("Patch document cannot be null.");
        
        var tournament = await _tournamentService.GetAsync(id);
        if (tournament == null) 
            return NotFound($"Tournament with ID {id} not found.");

        patchDocument.ApplyTo(tournament, ModelState);
        TryValidateModel(tournament);

        if (!ModelState.IsValid)            
            return UnprocessableEntity(ModelState);

        await _tournamentService.UpdateAsync(id, tournament);

        return Ok(tournament);
    }     
}
