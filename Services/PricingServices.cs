using WorkspaceTenants.Models;
using System.Collections.Generic;

using System;

namespace WorkspaceTenants{

public class PricingService
{
    private readonly List<IPricingRule> _rules = new();

    public PricingService(IEnumerable<IPricingRule> rules)
    {
        _rules.AddRange(rules);
    }

    public decimal Calculate(Rooms room, Bookings booking, Guid tenantId)
    {
        var hours = (decimal)(booking.EndUtc - booking.StartUtc).TotalHours;
        var baseAmount = hours <= 0 ? 0m : Math.Max(hours, 1m) * room.HourlyRate;

        decimal amount = baseAmount;
        foreach (var r in _rules)
            amount = r.Apply(room, booking, amount, tenantId);

        return Math.Round(amount, 2);
    }
}
}
