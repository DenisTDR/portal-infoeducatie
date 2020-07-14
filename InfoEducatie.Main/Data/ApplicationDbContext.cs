using MCMS.Base.Data.TypeConfig;
using MCMS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace InfoEducatie.Main.Data
{
    public class ApplicationDbContext : BaseDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IOptions<EntitiesConfig> config)
            : base(options, config)
        {
        }
    }
}