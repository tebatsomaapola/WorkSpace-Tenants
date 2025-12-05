using WorkspaceTenants.Models;
using System;

namespace WorkspaceTenants{

public class LongBookingDiscountRule : IPricingRule
{
    public decimal Apply(Rooms room, Bookings booking, decimal currentAmount, Guid tenantId)
    {
        var hours = (decimal)(booking.EndUtc - booking.StartUtc).TotalHours;
        return hours >= 4m ? currentAmount * 0.90m : currentAmount;
    }
}
}
