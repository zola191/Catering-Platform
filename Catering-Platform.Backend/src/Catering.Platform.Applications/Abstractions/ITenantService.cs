﻿using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Requests.Tenant;

namespace Catering.Platform.Applications.Abstractions
{
    public interface ITenantService
    {
        Task<IEnumerable<TenantViewModel>> GetAllAsync();
        Task<TenantViewModel?> GetByIdAsync(Guid id);
        Task<Guid> AddAsync(CreateTenantRequest request);
        Task<Guid> UpdateAsync(Guid id, UpdateTenantRequest request);
        Task DeleteAsync(Guid id);
        Task<TenantViewModel> BlockTenantAsync(Guid id, BlockTenantRequest request);
        Task<TenantViewModel> UnblockTenantAsync(Guid tenantId);
    }
}
