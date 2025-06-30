using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tournament.Core.DTO;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;

namespace Tournament.Data.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController(IUnitOfWork unitOfWork, IMapper mapper) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;


    // GET: api/Games
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameDTO>>> GetGame()
    {
        var games = await _unitOfWork.GameRepository.GetAllAsync();
        var dto = _mapper.Map<IEnumerable<Game>>(games);
        return Ok(dto);
    }


    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<GameDTO>>> GetGamesByTitle(string title)
    {
        var games = await _unitOfWork.GameRepository.GetByTitleAsync(title);
        if (!games.Any())
        {
            return NotFound($"No games with title: {title} found!");
        }
        var dto = _mapper.Map<IEnumerable<GameDTO>>(games);
        return Ok(dto);
    }


    // GET: api/Games/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Game>> GetGame(int id)
    {
        var dto = await _unitOfWork.GameRepository.GetAsync(id);

        if (dto == null)
        {
            return NotFound($"Game with ID {id} not found.");
        }

        return Ok(_mapper.Map<GameDTO>(dto));

    }


    // PUT: api/Games/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGame(int id, GameDTO dto)
    {
        if (!await _unitOfWork.GameRepository.AnyAsync(id))            
            return NotFound($"Game with ID {id} not found.");
        
        var entity = await _unitOfWork.GameRepository.GetAsync(id);
        if (entity == null)            
            return NotFound($"Game with ID {id} not found.");

        entity.Title = dto.Title;
        entity.Time = dto.Time;

        _unitOfWork.GameRepository.Update(entity);
        await _unitOfWork.CompleteAsync();

        return NoContent();

    }


    // POST: api/Games
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Game>> PostGame(GameDTO dto)
    {
        // Validate the DTO before mapping
        var entity = _mapper.Map<Game>(dto);
        _unitOfWork.GameRepository.Add(entity);
        await _unitOfWork.CompleteAsync();

        // Mapping the entity back to DTO for the response
        var createdDto = _mapper.Map<GameDTO>(entity);
        return CreatedAtAction(nameof(GetGame), new { id = entity.Id }, createdDto);
    }


    // DELETE: api/Games/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var dto = await _unitOfWork.GameRepository.GetAsync(id);
        if (dto == null)
        {
            return NotFound($"Couldn't delete. Game with ID {id} not found.");
        }

        _unitOfWork.GameRepository.Remove(dto);
        await _unitOfWork.CompleteAsync();

        return NoContent();

    }


    [HttpPatch("{id:int}")]
    public async Task<ActionResult<GameDTO>> PatchGame(int id, JsonPatchDocument<GameDTO> patchDocument)
    {
        if (patchDocument == null)
            return BadRequest("Patch document cannot be null.");

        var existingGame = await _unitOfWork.GameRepository.GetAsync(id);
        if (existingGame == null)
            return NotFound($"Game with ID {id} not found.");

        var dto = _mapper.Map<GameDTO>(existingGame);
        patchDocument.ApplyTo(dto, ModelState);
        TryValidateModel(dto);

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        _mapper.Map(dto, existingGame);
        //_unitOfWork.GameRepository.Update(existingGame);
        await _unitOfWork.CompleteAsync();

        return Ok(dto);

    }
}
