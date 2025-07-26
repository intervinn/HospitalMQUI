using HospitalCommon;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HospitalMQ.Models
{
    public class AppDbContext : DbContext
    {
        private IConfiguration _config;

        public AppDbContext(IConfiguration config)
        {
            _config = config;
        }

        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Doctor> Doctors => Set<Doctor>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_config.GetConnectionString("Sqlite"));
        }
    }
}
