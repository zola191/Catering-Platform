using Catering.Platform.Domain.Models;
using MediatR;

namespace Catering.Platform.Applications.Features.Categories.GetAll;

public class GetAllQuery : IRequest<IEnumerable<Category>>
{

}
