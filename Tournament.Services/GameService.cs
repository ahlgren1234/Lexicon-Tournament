using AutoMapper;
using Service.Contracts.DTO;
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

    public async Task<IEnumerable<GameDTO>> GetAllAsync()
    {
        var games = await _unitOfWork.GameRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<GameDTO>>(games);
    }

    public async Task<GameDTO?> GetAsync(int id)
    {
        var game = await _unitOfWork.GameRepository.GetAsync(id);
        return game == null ? null : _mapper.Map<GameDTO>(game);
    }

    public async Task<GameDTO> AddAsync(GameDTO gameDTO)
    {
        var game = _mapper.Map<Game>(gameDTO);
        _unitOfWork.GameRepository.Add(game);
        await _unitOfWork.CompleteAsync();
        
        return _mapper.Map<GameDTO>(game);
    }

    public async Task<bool> UpdateAsync(int id, GameDTO gameDTO)
    {
        var game = await _unitOfWork.GameRepository.GetAsync(id);
        if (game == null) return false;

        _mapper.Map(gameDTO, game);
        _unitOfWork.GameRepository.Update(game);
        await _unitOfWork.CompleteAsync();
        
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var game = await _unitOfWork.GameRepository.GetAsync(id);
        if (game == null) return false;

        _unitOfWork.GameRepository.Remove(game);
        await _unitOfWork.CompleteAsync();
        
        return true;
    }
}
