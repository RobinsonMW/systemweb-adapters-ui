// MIT License.

using System.Web.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SystemWebAdapters.HttpHandlers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WebForms.Compiler.Dynamic;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebFormsCompilerExtensions
{
    public static IWebFormsBuilder AddPersistentWebFormsCompilation(this IWebFormsBuilder builder)
    {
        builder.Services.AddWebFormsCompilationCore(options => { });
        builder.Services.AddSingleton<PersistentSystemWebCompilation>();
        builder.Services.AddSingleton<IWebFormsCompiler>(ctx => ctx.GetRequiredService<PersistentSystemWebCompilation>());

        return builder;
    }

    /// <summary>
    /// Registers a <paramref name="tagPrefix"/> for use in compilation. This will ensure the assembly is hooked up for compilation
    /// as well as the namespace of <typeparamref name="T"/> is available for the registered prefix.
    /// </summary>
    /// <typeparam name="T">Type whose namespace is registered</typeparam>
    /// <param name="builder">The <see cref="IWebFormsBuilder"/>.</param>
    /// <param name="tagPrefix">The prefix to register.</param>
    /// <returns></returns>
    public static IWebFormsBuilder AddPrefix<T>(this IWebFormsBuilder builder, string tagPrefix)
        where T : Control
    {
        builder.Services.AddOptions<PageCompilationOptions>()
            .Configure(options =>
            {
                options.AddTypeNamespace<T>(tagPrefix);
            });

        return builder;
    }

    public static IWebFormsBuilder AddDynamicPages(this IWebFormsBuilder services)
        => services.AddDynamicPages(options => { });

    public static IWebFormsBuilder AddDynamicPages(this IWebFormsBuilder services, Action<PageCompilationOptions> configure)
    {
        services.Services.AddWebFormsCompilationCore(configure);
        services.Services.AddHostedService<WebFormsCompilationService>();
        services.Services.AddSingleton<DynamicSystemWebCompilation>();
        services.Services.AddSingleton<IWebFormsCompiler>(ctx => ctx.GetRequiredService<DynamicSystemWebCompilation>());
        services.Services.AddSingleton<IHttpHandlerCollection>(ctx => ctx.GetRequiredService<DynamicSystemWebCompilation>());

        return services;
    }

    private static void AddWebFormsCompilationCore(this IServiceCollection services, Action<PageCompilationOptions> configure)
    {
        services.AddOptions<PageCompilationOptions>()
            .Configure<IHostEnvironment>((options, env) =>
            {
                options.AddParser<PageParser>(".aspx");
                options.AddParser<MasterPageParser>(".Master");

                //TODO https://github.com/twsouthwick/systemweb-adapters-ui/issues/19 , keeping the code to tackle in next CR
                //options.AddParser<UserControlParser>(".ascx");
            })
            .Configure(configure);

        services.AddOptions<PagesSection>()
            .Configure<IOptions<PageCompilationOptions>>((options, compilation) =>
            {
                foreach (var known in compilation.Value.KnownTags)
                {
                    options.DefaultTagNamespaceRegisterEntries.Add(known);
                }

                options.EnableSessionState = System.Web.Configuration.PagesEnableSessionState.True;
            });
    }
}
