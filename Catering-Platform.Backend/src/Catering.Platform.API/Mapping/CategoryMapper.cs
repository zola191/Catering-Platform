using Catering.Platform.API.Contracts;
using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Models;

namespace Catering.Platform.API.Mapping;

public class CategoryMapper : IMapper<Category, CategoryViewModel>
{
    public CategoryViewModel MapToModel(Category entity)
    {
        return new CategoryViewModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description
        };
    }

    public Category MapToEntity(CategoryViewModel model)
    {
        return new Category
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description
        };
    }
}