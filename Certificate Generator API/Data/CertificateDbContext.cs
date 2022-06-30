using Microsoft.EntityFrameworkCore;

namespace CertificateGeneratorAPI.Data
{
    public class CertificateDbContext : DbContext
    {
        public CertificateDbContext(DbContextOptions<CertificateDbContext> options) : base(options)
        {

        }

        public DbSet<Holder> Holders { get; set; }  
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<CertificateType> CertificateTypes { get; set; }
    }
}
