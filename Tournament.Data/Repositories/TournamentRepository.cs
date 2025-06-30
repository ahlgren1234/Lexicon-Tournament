using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Repositories;
using Tournament.Core.Entities;
using Tournament.Data.Data;

namespace Tournament.Data.Repositories
{
    public class TournamentRepository(TournamentContext context) : ITournamentRepository
    {
        private readonly TournamentContext _context = context;

        public async Task<IEnumerable<TournamentDetails>> GetAllAsync(bool includeGames)
        {
            if (includeGames)
            {
                return await _context.TournamentDetails
                    .Include(t => t.Games)
                    .ToListAsync();
            }
            return await _context.TournamentDetails.ToListAsync();

        }

        public async Task<TournamentDetails?> GetAsync(int id, bool includeGames = false)
        {
            IQueryable<TournamentDetails> query = _context.TournamentDetails;
            if (includeGames)
                query = query.Include(t => t.Games);

            return await query.FirstOrDefaultAsync(t => t.Id == id);

            //return await _context.TournamentDetails
            //.Include(t => t.Games)
            //.FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task<bool> AnyAsync(int id) => await _context.TournamentDetails.AnyAsync(t => t.Id == id);
        //{
            //return await _context.TournamentDetails.AnyAsync(t => t.Id == id);
        //}

        public void Add(TournamentDetails tournament) => _context.TournamentDetails.Add(tournament);
        //{
            //_context.TournamentDetails.Add(tournament);
        //}

        public void Update(TournamentDetails tournament) => _context.TournamentDetails.Update(tournament);
        //{
        //_context.TournamentDetails.Update(tournament);
        //}

        public void Remove(TournamentDetails tournament) => _context.TournamentDetails.Remove(tournament);
        //{
        //_context.TournamentDetails.Remove(tournament);
        //}

    }
}
