using Service.Contracts.Services;

namespace Service.Contracts.Services;

public interface IServiceManager
{
    ITournamentService TournamentService { get; }
    IGameService GameService { get; }
}
