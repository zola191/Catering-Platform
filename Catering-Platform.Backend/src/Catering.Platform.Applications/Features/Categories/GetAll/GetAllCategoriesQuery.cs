using Catering.Platform.Domain.Models;
using MediatR;

namespace Catering.Platform.Applications.Features.Categories.GetAll;

public record GetAllCategoriesQuery : IRequest<IEnumerable<Category>>
{

}
