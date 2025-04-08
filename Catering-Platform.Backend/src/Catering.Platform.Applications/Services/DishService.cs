using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Catering.Platform.Applications.Services;

public class DishService : IDishService
{
    private readonly IDishRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<IDishService> _logger;
    
    public DishService(IDishRepository repository, IUnitOfWork unitOfWork, ILogger<IDishService> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public Task<List<Dish>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return _repository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Unable to fetch all dishes. See Details: {Details}", ex.Message);
            throw;
        }
    }

    public Task<Dish?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return _repository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Unable to fetch dish by id {Id}. See Details: {Details}", id, ex.Message);
            throw;
        }
    }

    public async Task<Guid> AddAsync(Dish entity, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Unable to save dish {Name}, {Description}. See Details: {Details}",
                entity.Name,
                entity.Description,
                ex.Message);
            throw;
        }
    }

    public async Task<Guid> UpdateAsync(Dish entity, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Unable to update dish {Name}, {Description}. See Details: {Details}",
                entity.Name,
                entity.Description,
                ex.Message);
            throw;
        }
    }

    public async Task DeleteAsync(Dish entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Unable to delete dish {Name}, {Description}. See Details: {Details}",
                entity.Name,
                entity.Description,
                ex.Message);
            throw;
        }
    }
}