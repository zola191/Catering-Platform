using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests;
using Catering.Platform.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Catering.Platform.Applications.Services;

internal sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CategoryService> _logger;
    public CategoryService(ICategoryRepository repository, IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    // CategoryViewModel
    public async Task<Guid> AddAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var category = new Category()
            {
                Name = request.Name,
                Description = request.Description
            };
            var result = await _repository.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Unable to save category {Name}, {Description}. See Details: {Details}",
                request.Name,
                request.Description,
                ex.Message);
            throw; // чтобы не терять callstack
        }
    }

    public async Task DeleteAsync(Category entity, CancellationToken cancellationToken)
    {
        try
        {
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
            "Unable to delete category {Name}, {Description}. See Details: {Details}",
            entity.Name,
            entity.Description,
            ex.Message);
            throw; // чтобы не терять callstack
        }
    }

    public Task<List<Category>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            //пока не нужен await, после добавления ViewModel потребуется материализация
            return _repository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
            "Unable to fetch all categories. See Details: {Details}", ex.Message);
            throw; // чтобы не терять callstack
        }
    }

    // минусы интерполяции, логгер для каждого вхождения будет создавать
    // отдельную подстроку что съедает память для форматирования, актуально когда logdebagger
    // с placeholder logger работает более оптимально

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return _repository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
            "Unable to fetch category by id {Id}. See Details: {Details}", id, ex.Message);
            throw;
        }
    }

    public async Task<Guid> UpdateAsync(Category entity, CancellationToken cancellationToken)
    {
        try
        {
            var result = _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            // добавить полный слепок как в AddAsync, с указанием старого состояния и нового состояния,
            // актуально для финтех/медицинские где цена ошибки высока
            _logger.LogError(
                "Unable to update category {Name}, {Description}. See Details: {Details}",
                entity.Name,
                entity.Description,
                ex.Message);
            throw;
        }
    }
}
