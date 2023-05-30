using aspnet02_boardapp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace aspnet02_boardapp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Board> Boards { get; set; }

        //포트폴리오를 DB로 관리하기 위한 모델
        public DbSet<PortfolioModel> Portfolios { get; set; }
    }
}
