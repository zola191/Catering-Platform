using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using MediatR;

namespace Catering.Platform.Applications.Features.Categories.GetAll;

public class GetAllQueryHandler : IRequestHandler<GetAllQuery, IEnumerable<Category>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetAllQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<Category>> Handle(GetAllQuery request, CancellationToken cancellationToken)
    {
        return await _categoryRepository.GetAllAsync(cancellationToken);
    }
}
