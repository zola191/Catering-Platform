﻿namespace Catering.Platform.Domain.Models;

public class Tenant : Entity
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? BlockReason { get; set; }
    public List<Address> Addresses { get; set; } = new List<Address>();
}
