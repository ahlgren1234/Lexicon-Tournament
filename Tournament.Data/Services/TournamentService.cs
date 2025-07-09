using AutoMapper;
using Service.Contracts.DTO;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Service.Contracts.Services;

namespace Tournament.Data.Services;

public class TournamentService : ITournamentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TournamentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TournamentDTO>> GetAllAsync(bool includeGames = false)
    {
        var tournaments = await _unitOfWork.TournamentRepository.GetAllAsync(includeGames);
        return _mapper.Map<IEnumerable<TournamentDTO>>(tournaments);
    }

    public async Task<TournamentDTO?> GetAsync(int id, bool includeGames = false)
    {
        var tournament = await _unitOfWork.TournamentRepository.GetAsync(id, includeGames);
        return tournament == null ? null : _mapper.Map<TournamentDTO>(tournament);
    }

    public async Task<TournamentDTO> AddAsync(TournamentDTO tournamentDTO)
    {
        var tournament = _mapper.Map<TournamentDetails>(tournamentDTO);
        _unitOfWork.TournamentRepository.Add(tournament);
        await _unitOfWork.CompleteAsync();
        
        return _mapper.Map<TournamentDTO>(tournament);
    }

    public async Task<bool> UpdateAsync(int id, TournamentDTO tournamentDTO)
    {
        var tournament = await _unitOfWork.TournamentRepository.GetAsync(id);
        if (tournament == null) return false;

        _mapper.Map(tournamentDTO, tournament);
        _unitOfWork.TournamentRepository.Update(tournament);
        await _unitOfWork.CompleteAsync();
        
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tournament = await _unitOfWork.TournamentRepository.GetAsync(id);
        if (tournament == null) return false;

        _unitOfWork.TournamentRepository.Remove(tournament);
        await _unitOfWork.CompleteAsync();
        
        return true;
    }
}
