namespace Catering.Platform.Domain.Requests.Company;

public class GetCompaniesRequest
{
    public Guid? TenantId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
