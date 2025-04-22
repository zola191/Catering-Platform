using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catering.Platform.Applications.Features.Categories.GetById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Category?>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<GetCategoryByIdQueryHandler> _logger;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, ILogger<GetCategoryByIdQueryHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<Category?> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            var category = await _categoryRepository.GetByIdAsync(query.Id, cancellationToken);
            if (category == null)
            {
                throw new CategoryNotFoundException();
            }

            return category;
        }

        catch (CategoryNotFoundException ex)
        {
            _logger.LogError(
                "Category does not exist {Id}: {Details}",
                query.Id,
                ex.Message);
            throw;
        }

        catch (Exception ex)
        {
            _logger.LogError(
                "Unable to fetch category by id {Id}. See Details: {Details}", query.Id, ex.Message);
            throw;
        }
    }
}