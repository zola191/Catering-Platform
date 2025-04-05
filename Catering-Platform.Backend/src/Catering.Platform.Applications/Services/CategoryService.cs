using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Catering.Platform.Applications.Services;

internal sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly IUnidOfWork _unidOfWork;
    private readonly ILogger<CategoryService> _logger;
    public CategoryService(ICategoryRepository repository, IUnidOfWork unidOfWork, ILogger<CategoryService> logger)
    {
        _repository = repository;
        _unidOfWork = unidOfWork;
        _logger = logger;
    }
    //CategoryViewModel
    public async Task<Guid> AddAsync(Category entity, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _repository.AddAsync(entity, cancellationToken);
            await _unidOfWork.SaveChangesAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Unable to save category {Name}, {Description}. See Details: {Details}",
                entity.Name,
                entity.Description,
                ex.Message);
            throw; // чтобы не терять callstack
        }
    }

    public async Task DeleteAsync(Category entity, CancellationToken cancellationToken)
    {
        try
        {
            _repository.Delete(entity);
            await _unidOfWork.SaveChangesAsync(cancellationToken);
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

    //минусы интерполяции, логгер для каждого вхождения будет создавать отдельную подстроку что съедает память для форматирования, актуально когда logdebagger
    //с placeholder logger работает более оптимально

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
            await _unidOfWork.SaveChangesAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            // добавить полный слепок как в AddAsync, с указанием старого состояния и нового состояния,
            // актуально для финтех/медицинские где цена ошибки высока
            _logger.LogError(
            "Unable to update category by id {Id}. See Details: {Details}", entity.Id, ex.Message);
            throw;
        }
    }
}
