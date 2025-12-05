using System;

namespace WorkspaceTenants.DTOs{

public record RoomAvailabilityQuery(Guid WorkspaceId, DateTime FromUtc, DateTime ToUtc, int? MinCapacity, decimal? MaxRate);
public record RoomAvailabilityItem(Guid RoomId, string RoomName, int Capacity, decimal HourlyRate);
}