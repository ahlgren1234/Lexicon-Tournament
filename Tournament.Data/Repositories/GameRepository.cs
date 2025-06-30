using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;

namespace Tournament.Data.Repositories
{
    public class GameRepository(TournamentContext context) : IGameRepository
    {
        private readonly TournamentContext _context = context;

        public async Task<IEnumerable<Game>> GetByTitleAsync(string title) => await _context.Games
            .Where(g => g.Title.Contains(title))
            .ToListAsync();

        //{
            //return await _context.Games
                //.Where(g => g.Title!.Contains(title!))
                //.ToListAsync();
        //}
        public async Task<IEnumerable<Game>> GetAllAsync() => await _context.Games
            .ToListAsync();

        //{
        //return await _context.Games.ToListAsync();
        //}

        public async Task<Game?> GetAsync(int id) => await _context.Games
            .FirstOrDefaultAsync(g => g.Id == id);

        //{
        //return await _context.Games                
        //.FirstOrDefaultAsync(g => g.Id == id);
        //}

        public async Task<bool> AnyAsync(int id) => await _context.Games.AnyAsync(g => g.Id == id);
        //{
        //return await _context.Games.AnyAsync(g => g.Id == id);
        //}

        public void Add(Game game) => _context.Games.Add(game);
        //{ 
        //_context.Games.Add(game); 
        //}
        public void Update(Game game) => _context.Games.Update(game);
        //{
        //_context.Games.Update(game);
        //}

        public void Remove(Game game) => _context.Games.Remove(game);
        //{
        //_context.Games.Remove(game);
        //}
    }
}
