using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Services.DTO;
using Service.Contracts.Services;
using Microsoft.AspNetCore.JsonPatch;

namespace Tournament.Api.Controllers;

[Route("api/TournamentDetails")]
[ApiController]
public class TournamentDetailsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public TournamentDetailsController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    // GET: api/TournamentDetails
    [HttpGet]
    public async Task<ActionResult<object>> GetTournamentDetails(
        [FromQuery] bool includeGames = false,
        [FromQuery] string? title = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _serviceManager.TournamentService.GetAllAsync(includeGames);
        if (!result.Success)
        {
            var problem = new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = 500,
                Detail = result.ErrorMessage
            };
            Response.ContentType = "application/json";
            return StatusCode(500, problem);
        }
        var tournaments = result.Data ?? new List<TournamentDTO>();

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
        int maxPageSize = 100;
        pageSize = pageSize > maxPageSize ? maxPageSize : (pageSize > 0 ? pageSize : 20);
        int totalItems = tournaments.Count();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        page = page < 1 ? 1 : (page > totalPages ? totalPages : page);
        var items = tournaments.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var response = new {
            items,
            metadata = new {
                totalPages,
                pageSize,
                currentPage = page,
                totalItems
            }
        };
        return Ok(response);
    }

    // GET: api/TournamentDetails/5 
    [HttpGet("{id}")]
    public async Task<ActionResult<TournamentDTO>> GetTournamentById(int id)
    {
        var result = await _serviceManager.TournamentService.GetAsync(id);
        if (!result.Success)
        {
            var problem = new ProblemDetails
            {
                Title = "Not Found",
                Status = 404,
                Detail = result.ErrorMessage
            };
            Response.ContentType = "application/json";
            return NotFound(problem);
        }
        return Ok(result.Data);
    }

    // POST: api/TournamentDetails
    [HttpPost]
    public async Task<ActionResult<TournamentDTO>> PostTournament(TournamentDTO tournamentDTO)
    {
        var result = await _serviceManager.TournamentService.AddAsync(tournamentDTO);
        if (!result.Success)
        {
            var problem = new ProblemDetails
            {
                Title = "Bad Request",
                Status = 400,
                Detail = result.ErrorMessage
            };
            Response.ContentType = "application/json";
            return BadRequest(problem);
        }
        return CreatedAtAction(nameof(GetTournamentById), new { id = result.Data?.Id }, result.Data);
    }

    // PUT: api/TournamentDetails/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTournament(int id, TournamentDTO tournamentDTO)
    {
        if (id != tournamentDTO.Id)
        {
            var problem = new ProblemDetails
            {
                Title = "Bad Request",
                Status = 400,
                Detail = "ID in URL does not match ID in body."
            };
            Response.ContentType = "application/json";
            return BadRequest(problem);
        }
        var result = await _serviceManager.TournamentService.UpdateAsync(id, tournamentDTO);
        if (!result.Success)
        {
            var problem = new ProblemDetails
            {
                Title = "Not Found",
                Status = 404,
                Detail = result.ErrorMessage
            };
            Response.ContentType = "application/json";
            return NotFound(problem);
        }
        return NoContent();
    }

    // DELETE: api/TournamentDetails/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTournament(int id)
    {
        var result = await _serviceManager.TournamentService.DeleteAsync(id);
        if (!result.Success)
        {
            var problem = new ProblemDetails
            {
                Title = "Not Found",
                Status = 404,
                Detail = result.ErrorMessage
            };
            Response.ContentType = "application/json";
            return NotFound(problem);
        }
        return NoContent();
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<TournamentDTO>> PatchTournament(int id, JsonPatchDocument<TournamentDTO> patchDocument)
    {
        if (patchDocument == null)
        {
            var problem = new ProblemDetails
            {
                Title = "Bad Request",
                Status = 400,
                Detail = "Patch document cannot be null."
            };
            Response.ContentType = "application/json";
            return BadRequest(problem);
        }
        var getResult = await _serviceManager.TournamentService.GetAsync(id);
        if (!getResult.Success || getResult.Data == null)
        {
            var problem = new ProblemDetails
            {
                Title = "Not Found",
                Status = 404,
                Detail = getResult.ErrorMessage ?? $"Tournament with ID {id} not found."
            };
            Response.ContentType = "application/json";
            return NotFound(problem);
        }
        var tournament = getResult.Data;
        patchDocument.ApplyTo(tournament, ModelState);
        TryValidateModel(tournament);
        if (!ModelState.IsValid)
        {
            var problem = new ValidationProblemDetails(ModelState)
            {
                Title = "Validation Error",
                Status = 422
            };
            Response.ContentType = "application/json";
            return UnprocessableEntity(problem);
        }
        var updateResult = await _serviceManager.TournamentService.UpdateAsync(id, tournament);
        if (!updateResult.Success)
        {
            var problem = new ProblemDetails
            {
                Title = "Internal Server Error",
                Status = 500,
                Detail = updateResult.ErrorMessage
            };
            Response.ContentType = "application/json";
            return StatusCode(500, problem);
        }
        return Ok(tournament);
    }
}
