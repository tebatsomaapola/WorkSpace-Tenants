using WorkspaceTenants.Models;

namespace WorkspaceTenants{

public class PeakHoursRule : IPricingRule
{
    public decimal Apply(Rooms room, Bookings booking, decimal currentAmount, Guid tenantId)
    {
        var startHour = booking.StartUtc.Hour;
        var endHour = booking.EndUtc.Hour;
        bool peak = (startHour >= 8 && startHour < 10) || (endHour > 16 && endHour <= 18);
        return peak ? currentAmount * 1.20m : currentAmount;
    }
}
}
