using AutoMapper;
using Service.Contracts.Services.DTO;
using Service.Contracts.Services;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;

namespace Tournament.Services;

public class TournamentService : ITournamentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TournamentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<TournamentDTO>>> GetAllAsync(bool includeGames = false)
    {
        var tournaments = await _unitOfWork.TournamentRepository.GetAllAsync(includeGames);
        return ServiceResult<IEnumerable<TournamentDTO>>.Ok(_mapper.Map<IEnumerable<TournamentDTO>>(tournaments));
    }

    public async Task<ServiceResult<TournamentDTO?>> GetAsync(int id, bool includeGames = false)
    {
        var tournament = await _unitOfWork.TournamentRepository.GetAsync(id, includeGames);
        if (tournament == null)
            return ServiceResult<TournamentDTO?>.Fail($"Tournament with ID {id} not found.");
        return ServiceResult<TournamentDTO?>.Ok(_mapper.Map<TournamentDTO>(tournament));
    }

    public async Task<ServiceResult<TournamentDTO>> AddAsync(TournamentDTO tournamentDTO)
    {
        var tournament = _mapper.Map<TournamentDetails>(tournamentDTO);
        _unitOfWork.TournamentRepository.Add(tournament);
        await _unitOfWork.CompleteAsync();
        return ServiceResult<TournamentDTO>.Ok(_mapper.Map<TournamentDTO>(tournament));
    }

    public async Task<ServiceResult<bool>> UpdateAsync(int id, TournamentDTO tournamentDTO)
    {
        var tournament = await _unitOfWork.TournamentRepository.GetAsync(id);
        if (tournament == null)
            return ServiceResult<bool>.Fail($"Tournament with ID {id} not found.");
        _mapper.Map(tournamentDTO, tournament);
        _unitOfWork.TournamentRepository.Update(tournament);
        await _unitOfWork.CompleteAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var tournament = await _unitOfWork.TournamentRepository.GetAsync(id);
        if (tournament == null)
            return ServiceResult<bool>.Fail($"Tournament with ID {id} not found.");
        _unitOfWork.TournamentRepository.Remove(tournament);
        await _unitOfWork.CompleteAsync();
        return ServiceResult<bool>.Ok(true);
    }
}
