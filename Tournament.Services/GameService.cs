using AutoMapper;
using Service.Contracts.Services.DTO;
using Service.Contracts.Services;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;

namespace Tournament.Services;

public class GameService : IGameService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GameService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<GameDTO>>> GetAllAsync()
    {
        var games = await _unitOfWork.GameRepository.GetAllAsync();
        return ServiceResult<IEnumerable<GameDTO>>.Ok(_mapper.Map<IEnumerable<GameDTO>>(games));
    }

    public async Task<ServiceResult<GameDTO?>> GetAsync(int id)
    {
        var game = await _unitOfWork.GameRepository.GetAsync(id);
        if (game == null)
            return ServiceResult<GameDTO?>.Fail($"Game with ID {id} not found.");
        return ServiceResult<GameDTO?>.Ok(_mapper.Map<GameDTO>(game));
    }

    public async Task<ServiceResult<GameDTO>> AddAsync(GameDTO gameDTO)
    {
        // Kontrollera max 10 matcher per turnering
        var tournament = await _unitOfWork.TournamentRepository.GetAsync(gameDTO.TournamentId, true);
        if (tournament == null)
            return ServiceResult<GameDTO>.Fail($"Tournament with ID {gameDTO.TournamentId} not found.");
        if (tournament.Games != null && tournament.Games.Count >= 10)
            return ServiceResult<GameDTO>.Fail("A tournament cannot have more than 10 games.");

        var game = _mapper.Map<Game>(gameDTO);
        _unitOfWork.GameRepository.Add(game);
        await _unitOfWork.CompleteAsync();
        return ServiceResult<GameDTO>.Ok(_mapper.Map<GameDTO>(game));
    }

    public async Task<ServiceResult<bool>> UpdateAsync(int id, GameDTO gameDTO)
    {
        var game = await _unitOfWork.GameRepository.GetAsync(id);
        if (game == null)
            return ServiceResult<bool>.Fail($"Game with ID {id} not found.");
        _mapper.Map(gameDTO, game);
        _unitOfWork.GameRepository.Update(game);
        await _unitOfWork.CompleteAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var game = await _unitOfWork.GameRepository.GetAsync(id);
        if (game == null)
            return ServiceResult<bool>.Fail($"Game with ID {id} not found.");
        _unitOfWork.GameRepository.Remove(game);
        await _unitOfWork.CompleteAsync();
        return ServiceResult<bool>.Ok(true);
    }
}
