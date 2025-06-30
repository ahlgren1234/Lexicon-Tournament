using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tournament.Data.Data;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Core.DTO;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;

namespace Tournament.Data.Controllers;

[Route("api/TournamentDetails")]
[ApiController]
public class TournamentDetailsController(IUnitOfWork unitOfWork, IMapper mapper) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;


    // GET: api/TournamentDetails
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TournamentDTO>>> GetTournamentDetails(
        [FromQuery] bool includeGames = false,
        [FromQuery] string? title = null,
        [FromQuery] string? sortBy = null)
    {
        var tournaments = await _unitOfWork.TournamentRepository.GetAllAsync(includeGames);

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

        var dto = _mapper.Map<IEnumerable<TournamentDTO>>(tournaments);
        return Ok(dto);
    }


    // GET: api/TournamentDetails/5 
    [HttpGet("{id}")]
    public async Task<ActionResult<TournamentDTO>> GetTournamentDetails(int id)
    {

        var dto = await _unitOfWork.TournamentRepository.GetAsync(id);

        if (dto == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<TournamentDTO>(dto));
    }


    // PUT: api/TournamentDetails/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTournamentDetails(int id, TournamentDTO dto)
    {
        if (!await _unitOfWork.TournamentRepository.AnyAsync(id))
            return NotFound();

        var entity = await _unitOfWork.TournamentRepository.GetAsync(id);
        if (entity == null)
            return NotFound();

        entity.Title = dto.Title;
        entity.StartDate = dto.StartDate;

        _unitOfWork.TournamentRepository.Update(entity);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }


    // POST: api/TournamentDetails
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TournamentDetails>> PostTournamentDetails(TournamentDTO dto)
    {
        var entity = _mapper.Map<TournamentDetails>(dto);
        _unitOfWork.TournamentRepository.Add(entity);
        await _unitOfWork.CompleteAsync();

        var createdDto = _mapper.Map<TournamentDTO>(entity);
        return CreatedAtAction(nameof(GetTournamentDetails), new { id = entity.Id }, createdDto);

    }


    // DELETE: api/TournamentDetails/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTournamentDetails(int id)
    {
        var dto = await _unitOfWork.TournamentRepository.GetAsync(id);
        if (dto == null)
        {
            return NotFound();
        }

        _unitOfWork.TournamentRepository.Remove(dto);
        await _unitOfWork.CompleteAsync();

        return NoContent();

    }


    [HttpPatch("{id:int}")]
    public async Task<ActionResult<TournamentDTO>> PatchTournament(int id, JsonPatchDocument<TournamentDTO> patchDocument)
    {
        if (patchDocument == null)            
            return BadRequest("Patch document cannot be null.");
        
        var tournamentExists = await _unitOfWork.TournamentRepository.GetAsync(id);
        if (tournamentExists == null) 
            return NotFound($"Tournament with ID {id} not found.");

        var dto = _mapper.Map<TournamentDTO>(tournamentExists);
        patchDocument.ApplyTo(dto, ModelState);
        TryValidateModel(dto);

        if (!ModelState.IsValid)            
            return UnprocessableEntity(ModelState);

        _mapper.Map(dto, tournamentExists);
        //_unitOfWork.TournamentRepository.Update(tournamentExists);
        await _unitOfWork.CompleteAsync();

        return Ok(dto);

    }     
}
