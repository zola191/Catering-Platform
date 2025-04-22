using Catering.Platform.Applications.Features.Categories.Create;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catering.Platform.Applications.Features.Categories.Update;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public UpdateCategoryCommandHandler(
        ICategoryRepository repository, 
        IUnitOfWork unitOfWork, 
        ILogger<CreateCategoryCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingCategory = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (existingCategory == null)
            {
                throw new CategoryNotFoundException();
            }
            UpdateCategoryCommand.UpdateEntity(existingCategory, request);
            
            var result = _repository.Update(existingCategory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return result;
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
            _logger.LogError(
                "Unable to update category {Name}, {Description}. See Details: {Details}",
                request.Name,
                request.Description,
                ex.Message);
            throw;
        }
    }
}