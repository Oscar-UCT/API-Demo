using Microsoft.EntityFrameworkCore;

class RecordatoriosDb : DbContext
{
    public RecordatoriosDb(DbContextOptions options) : base(options) { }
    public DbSet<Recordatorio> Recordatorios { get; set; } = null!;
}