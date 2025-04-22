using Catering.Platform.Applications.Features.Categories.Create;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catering.Platform.Applications.Features.Categories.Delete;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Guid>
{
    private readonly ICategoryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public DeleteCategoryCommandHandler(ICategoryRepository repository, IUnitOfWork unitOfWork, ILogger<CreateCategoryCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var existingCategory = await _repository.GetByIdAsync(request.Id, cancellationToken);
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
            throw;
        }
    }
}