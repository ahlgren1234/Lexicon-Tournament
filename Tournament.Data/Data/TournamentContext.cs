﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.Entities;

namespace Tournament.Data.Data;

public class TournamentContext(DbContextOptions<TournamentContext> options) : DbContext(options)
{
    public DbSet<Tournament.Core.Entities.TournamentDetails> TournamentDetails { get; set; } = default!;
    public DbSet<Tournament.Core.Entities.Game> Games { get; set; } = default!;
}
