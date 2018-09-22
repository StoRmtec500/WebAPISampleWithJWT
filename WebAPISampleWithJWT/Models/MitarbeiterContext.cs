using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPISampleWithJWT.Models
{
    public class MitarbeiterContext : DbContext
    {
        public MitarbeiterContext(DbContextOptions<MitarbeiterContext> options) : base(options) { }

        public DbSet<Mitarbeiter> Mitarbeiter { get; set; }
    }
}
