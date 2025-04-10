using Catering.Platform.Domain.Exceptions;
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

    public async Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var existingCategory = await _repository.GetByIdAsync(id, cancellationToken);
        if (existingCategory == null)
        {
            throw new CategoryNotFoundException();
        }
        try
        {
            _repository.Delete(existingCategory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return existingCategory.Id;
        }

        catch (CategoryNotFoundException ex)
        {
            _logger.LogError(
            "Category does not exist {Name}, {Description}. See Details: {Details}",
            existingCategory.Name,
            existingCategory.Description,
            ex.Message);
            throw;
        }

        catch (Exception ex)
        {
            _logger.LogError(
            "Unable to delete category {Name}, {Description}. See Details: {Details}",
            existingCategory.Name,
            existingCategory.Description,
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

    public async Task<Guid> UpdateAsync(
        Guid id,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var existingCategory = await _repository.GetByIdAsync(id, cancellationToken);
            if (existingCategory != null)
            {
                existingCategory.Name = request.Name;
                existingCategory.Description = request.Description;
                var result = _repository.Update(existingCategory);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return result;
            }
            throw new CategoryNotFoundException();
        }

        catch (CategoryNotFoundException ex)
        {
            _logger.LogError(
            "Category does not exist {Name}, {Description}. See Details: {Details}",
            request.Name,
            request.Description,
            ex.Message);
            throw;
        }

        catch (Exception ex)
        {
            // добавить полный слепок как в AddAsync, с указанием старого состояния и нового состояния,
            // актуально для финтех/медицинские где цена ошибки высока
            _logger.LogError(
                "Unable to update category {Name}, {Description}. See Details: {Details}",
                request.Name,
                request.Description,
                ex.Message);
            throw;
        }

    }
}
