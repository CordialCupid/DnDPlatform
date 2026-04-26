using DnDPlatform.Models.Domain;
using DnDPlatform.Repositories.Data;
using DnDPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DnDPlatform.Repositories.Implementations;

public class EfTemplateRepository : ITemplateRepository
{
    private readonly DnDDbContext _db;
    public EfTemplateRepository(DnDDbContext db)
    {
        _db = db;
    }
    public Task<IEnumerable<Template>> GetAllAsync(string? system = null)
    {
        IQueryable<Template> query = _db.Templates;
        if (!string.IsNullOrWhiteSpace(system))
        {
            query = query.Where(t => t.System == system);         
        }
        return Task.FromResult<IEnumerable<Template>>(query.AsEnumerable());
    }

    public Task<Template?> GetByIdAsync(Guid id, bool includeFields = false)
    {
        IQueryable<Template> query = _db.Templates;
        if (includeFields)
        {
            query = query.Include(t => t.FieldDefinitions);   
        }
        return query.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Template> InsertAsync(Template template)
    {
        _db.Templates.Add(template);
        await _db.SaveChangesAsync();
        return template;
    }
}
