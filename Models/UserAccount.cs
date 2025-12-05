using System;

namespace WorkspaceTenants.Models{

public class UserAccount
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = ""; 
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
}
}
