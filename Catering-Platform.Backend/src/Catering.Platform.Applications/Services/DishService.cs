using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.Models;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests;
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
    
    public async Task<List<DishViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var existingDishes = await _repository.GetAllAsync(cancellationToken);
            var existingDishViewmodels = existingDishes.Select(DishViewModel.MapToViewModel).ToList();
            return existingDishViewmodels;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Unable to fetch all dishes. See Details: {Details}", ex.Message);
            throw;
        }
    }

    public async Task<DishViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingDish = await _repository.GetByIdAsync(id, cancellationToken);
            if (existingDish == null)
            {
                throw new DishNotFoundException();
            }
            return DishViewModel.MapToViewModel(existingDish);
        }

        catch (DishNotFoundException ex)
        {
            _logger.LogError(
                "Dish is not found {Id}. See Details: {Details}", id, ex.Message);
            throw;
        }

        catch (Exception ex)
        {
            _logger.LogError(
                "Unable to fetch dish by id {Id}. See Details: {Details}", id, ex.Message);
            throw;
        }
    }

    public async Task<Guid> AddAsync(CreateDishRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var dish = CreateDishRequest.MapToDomain(request);
            var result = await _repository.AddAsync(dish, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Unable to save dish {Name}, {Description}. See Details: {Details}",
                request.Name,
                request.Description,
                ex.Message);
            throw;
        }
    }

    public async Task<Guid> UpdateAsync(Guid id, UpdateDishRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingDish = await _repository.GetByIdAsync(id, cancellationToken);
            if (existingDish == null)
            {
                throw new DishNotFoundException();
            }
            existingDish = UpdateDishRequest.UpdateFrom(existingDish);
            var result = _repository.Update(existingDish);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result;
        }

        catch (DishNotFoundException ex)
        {
            _logger.LogError(
                "Dish is not found {Id}. See Details: {Details}", id, ex.Message);
            throw;
        }

        catch (Exception ex)
        {
            _logger.LogError(
                "Unable to update dish {Name}, {Description}. See Details: {Details}",
                request.Name,
                request.Description,
                ex.Message);
            throw;
        }
    }

    public async Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existingDish = await _repository.GetByIdAsync(id,cancellationToken);
        if (existingDish == null)
        {
            throw new DishNotFoundException();
        }
        try
        {
            _repository.Delete(existingDish);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return existingDish.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Unable to delete dish {Name}, {Description}. See Details: {Details}",
                existingDish.Name,
                existingDish.Description,
                ex.Message);
            throw;
        }
    }
}