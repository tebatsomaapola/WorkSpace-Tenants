using System;

namespace WorkspaceTenants.DTOs{

public record InvoiceResponse(Guid Id, decimal Amount, string Status);
}