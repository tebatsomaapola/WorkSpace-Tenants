using WorkspaceTenants.Models;
using System;
namespace WorkspaceTenants{

public interface IPricingRule
{
    decimal Apply(Rooms room, Bookings booking, decimal currentAmount, Guid tenantId);
}

}
