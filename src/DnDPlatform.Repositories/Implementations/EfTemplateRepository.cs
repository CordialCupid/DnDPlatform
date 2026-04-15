using DnDPlatform.Models.Domain;
using DnDPlatform.Repositories.Data;
using DnDPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DnDPlatform.Repositories.Implementations;

public class EfTemplateRepository(DnDDbContext db) : ITemplateRepository
{
    public Task<IEnumerable<Template>> GetAllAsync(string? system = null)
    {
        IQueryable<Template> query = db.Templates;
        if (!string.IsNullOrWhiteSpace(system))
            query = query.Where(t => t.System == system);
        return Task.FromResult<IEnumerable<Template>>(query.AsEnumerable());
    }

    public Task<Template?> GetByIdAsync(Guid id, bool includeFields = false)
    {
        IQueryable<Template> query = db.Templates;
        if (includeFields)
            query = query.Include(t => t.FieldDefinitions);
        return query.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Template> InsertAsync(Template template)
    {
        db.Templates.Add(template);
        await db.SaveChangesAsync();
        return template;
    }

    public async Task<Template> UpdateAsync(Template template)
    {
        db.Templates.Update(template);
        await db.SaveChangesAsync();
        return template;
    }

    public async Task DeleteAsync(Guid id)
    {
        var template = await db.Templates.FindAsync(id);
        if (template is not null)
        {
            db.Templates.Remove(template);
            await db.SaveChangesAsync();
        }
    }
}
