using System;
using System.Threading;
using System.Threading.Tasks;
using WorkspaceTenants.DTOs;
using WorkspaceTenants.Common;

namespace WorkspaceTenants.Services{

public interface IInvoiceService
{
    Task<(Result result, InvoiceResponse? invoice)> CreateDraftAsync(Guid bookingId, ITenantContext context, CancellationToken ct);
}
}