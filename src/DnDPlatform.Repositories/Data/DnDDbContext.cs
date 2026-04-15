using DnDPlatform.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace DnDPlatform.Repositories.Data;

public class DnDDbContext(DbContextOptions<DnDDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<CharacterSheet> CharacterSheets => Set<CharacterSheet>();
    public DbSet<Template> Templates => Set<Template>();
    public DbSet<FieldDefinition> FieldDefinitions => Set<FieldDefinition>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Username).IsUnique();
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(u => u.CreatedAt).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<Character>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(c => c.CreatedAt).HasDefaultValueSql("now()");
            e.Property(c => c.UpdatedAt).HasDefaultValueSql("now()");
            e.HasOne(c => c.Owner).WithMany(u => u.Characters).HasForeignKey(c => c.OwnerId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(c => c.Template).WithMany().HasForeignKey(c => c.TemplateId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CharacterSheet>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(s => s.CreatedAt).HasDefaultValueSql("now()");
            e.Property(s => s.JsonBlob).HasColumnType("jsonb");
            e.HasOne(s => s.Character).WithMany(c => c.Sheets).HasForeignKey(s => s.CharacterId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(s => new { s.CharacterId, s.VersionNumber }).IsUnique();
        });

        modelBuilder.Entity<Template>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(t => t.CreatedAt).HasDefaultValueSql("now()");
            e.Property(t => t.JsonSchema).HasColumnType("jsonb");
            e.Property(t => t.DefaultLayoutJson).HasColumnType("jsonb");
            e.HasOne(t => t.Owner).WithMany(u => u.Templates).HasForeignKey(t => t.OwnerId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FieldDefinition>(e =>
        {
            e.HasKey(f => f.Id);
            e.Property(f => f.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(f => f.ValidationRulesJson).HasColumnType("jsonb");
            e.HasOne(f => f.Template).WithMany(t => t.FieldDefinitions).HasForeignKey(f => f.TemplateId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AuditLog>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(a => a.Timestamp).HasDefaultValueSql("now()");
            e.Property(a => a.MetadataJson).HasColumnType("jsonb");
            e.ToTable("AuditLogs");
        });
    }
}
