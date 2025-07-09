using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Services.DTO;
using Service.Contracts.Services;

namespace Tournament.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public GamesController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    // GET: api/Games
    [HttpGet]
    public async Task<ActionResult<object>> GetGames([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _serviceManager.GameService.GetAllAsync();
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
        var games = result.Data ?? new List<GameDTO>();
        int maxPageSize = 100;
        pageSize = pageSize > maxPageSize ? maxPageSize : (pageSize > 0 ? pageSize : 20);
        int totalItems = games.Count();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        page = page < 1 ? 1 : (page > totalPages ? totalPages : page);
        var items = games.Skip((page - 1) * pageSize).Take(pageSize).ToList();
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

    // GET: api/Games/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GameDTO>> GetGame(int id)
    {
        var result = await _serviceManager.GameService.GetAsync(id);
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

    // PUT: api/Games/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGame(int id, GameDTO gameDTO)
    {
        if (id != gameDTO.Id)
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
        var result = await _serviceManager.GameService.UpdateAsync(id, gameDTO);
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

    // POST: api/Games
    [HttpPost]
    public async Task<ActionResult<GameDTO>> PostGame(GameDTO gameDTO)
    {
        var result = await _serviceManager.GameService.AddAsync(gameDTO);
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
        return CreatedAtAction(nameof(GetGame), new { id = result.Data?.Id }, result.Data);
    }

    // DELETE: api/Games/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var result = await _serviceManager.GameService.DeleteAsync(id);
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
    public async Task<ActionResult<GameDTO>> PatchGame(int id, JsonPatchDocument<GameDTO> patchDocument)
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
        var getResult = await _serviceManager.GameService.GetAsync(id);
        if (!getResult.Success || getResult.Data == null)
        {
            var problem = new ProblemDetails
            {
                Title = "Not Found",
                Status = 404,
                Detail = getResult.ErrorMessage ?? $"Game with ID {id} not found."
            };
            Response.ContentType = "application/json";
            return NotFound(problem);
        }
        var game = getResult.Data;
        patchDocument.ApplyTo(game, ModelState);
        TryValidateModel(game);
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
        var updateResult = await _serviceManager.GameService.UpdateAsync(id, game);
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
        return Ok(game);
    }
}
