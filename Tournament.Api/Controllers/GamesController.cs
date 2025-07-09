using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.DTO;
using Service.Contracts.Services;

namespace Tournament.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }


    // GET: api/Games
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameDTO>>> GetGames()
    {
        var games = await _gameService.GetAllAsync();
        return Ok(games);
    }

    // GET: api/Games/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GameDTO>> GetGame(int id)
    {
        var game = await _gameService.GetAsync(id);

        if (game == null)
        {
            return NotFound($"Game with ID {id} not found.");
        }

        return Ok(game);
    }

    // PUT: api/Games/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGame(int id, GameDTO gameDTO)
    {
        if (id != gameDTO.Id)
        {
            return BadRequest();
        }

        var result = await _gameService.UpdateAsync(id, gameDTO);
        
        if (!result)
        {
            return NotFound($"Game with ID {id} not found.");
        }

        return NoContent();
    }

    // POST: api/Games
    [HttpPost]
    public async Task<ActionResult<GameDTO>> PostGame(GameDTO gameDTO)
    {
        var createdGame = await _gameService.AddAsync(gameDTO);
        return CreatedAtAction(nameof(GetGame), new { id = createdGame.Id }, createdGame);
    }

    // DELETE: api/Games/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var result = await _gameService.DeleteAsync(id);
        
        if (!result)
        {
            return NotFound($"Couldn't delete. Game with ID {id} not found.");
        }

        return NoContent();
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<GameDTO>> PatchGame(int id, JsonPatchDocument<GameDTO> patchDocument)
    {
        if (patchDocument == null)
            return BadRequest("Patch document cannot be null.");

        var game = await _gameService.GetAsync(id);
        if (game == null)
            return NotFound($"Game with ID {id} not found.");

        patchDocument.ApplyTo(game, ModelState);
        TryValidateModel(game);

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        await _gameService.UpdateAsync(id, game);

        return Ok(game);
    }
}
