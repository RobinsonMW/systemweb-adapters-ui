// MIT License.

using System.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SystemWebAdapters;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class HandlerServicesExtensions
{
    public static void AddHttpHandlers(this ISystemWebAdapterBuilder services)
    {
        services.Services.TryAddSingleton<HttpHandlerEndpointCache>();
    }

    public static void UseHttpHandlers(this IApplicationBuilder app)
    {
        app.UseMiddleware<SetHttpHandlerMiddleware>();
    }
}
