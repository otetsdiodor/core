using CoreBattle.Domain.Core.GameDomain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBattle.Infrastructure.Data
{
    public class AppContext : DbContext
    {
        DbSet<Game> Games { get; set; }
        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}
