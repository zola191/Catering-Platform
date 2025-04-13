using Catering.Platform.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catering.Platform.Applications.Features.Categories.Create;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(
        ICategoryRepository repository, 
        IUnitOfWork unitOfWork, 
        ILogger<CreateCategoryCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var category = CreateCategoryCommand.MapToDomain(request);
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
}
