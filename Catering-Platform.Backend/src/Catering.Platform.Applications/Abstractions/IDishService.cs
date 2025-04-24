using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Requests.Dish;

namespace Catering.Platform.Applications.Abstractions;

public interface IDishService
{
    Task<List<DishViewModel>> GetAllAsync();
    Task<DishViewModel?> GetByIdAsync(Guid id);
    Task<Guid> AddAsync(CreateDishRequest request);
    Task<Guid> UpdateAsync(Guid id, UpdateDishRequest request);
    Task<Guid> DeleteAsync(Guid id);
}