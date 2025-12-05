using WorkspaceTenants.Models;
using System;

namespace WorkspaceTenants{

public class TenantDiscountRule : IPricingRule
{
    private readonly Func<Guid, decimal?> _tenantPercentProvider;

    public TenantDiscountRule(Func<Guid, decimal?> tenantPercentProvider)
    {
        _tenantPercentProvider = tenantPercentProvider;
    }

    public decimal Apply(Rooms room, Bookings booking, decimal currentAmount, Guid tenantId)
    {
        var pct = _tenantPercentProvider(tenantId) ?? 0m;
        return pct > 0m ? currentAmount * (1m - pct) : currentAmount;
  
    }
}
}
