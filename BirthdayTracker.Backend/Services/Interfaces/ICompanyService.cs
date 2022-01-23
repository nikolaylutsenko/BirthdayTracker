using BirthdayTracker.Shared.Entities;

namespace BirthdayTracker.Shared.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<Company> GetAsync(string id);
        Task<IEnumerable<Company>> GetAllAsync();
        Task DeleteAsync(string id);
        Task AddAsync(Company company);
        Task UpdateAsync(Company company);
    }
}
