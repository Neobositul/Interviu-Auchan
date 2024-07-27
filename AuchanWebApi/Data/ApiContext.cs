using Microsoft.EntityFrameworkCore;
using AuchanWebApi.Models;

namespace AuchanWebApi.Data
{
    public class ApiContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {

        }
    }
}
