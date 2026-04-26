using DnDPlatform.Models.Domain;

namespace DnDPlatform.Repositories.Interfaces;

public interface ITemplateRepository
{
    Task<IEnumerable<Template>> GetAllAsync(string? system = null);
    Task<Template?> GetByIdAsync(Guid id, bool includeFields = false);
    Task<Template> InsertAsync(Template template);
    //Task DeleteAsync(Guid id);
}
