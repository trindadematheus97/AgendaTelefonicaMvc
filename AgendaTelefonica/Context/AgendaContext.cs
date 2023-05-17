using AgendaTelefonica.Models;
using Microsoft.EntityFrameworkCore;

namespace AgendaTelefonica.Context
{
    public class AgendaContext : DbContext
    {
        public DbSet<Pessoa> Pessoas { get; set; }

        public AgendaContext(DbContextOptions<AgendaContext> options) : base(options)
        {

        }
    }
}
