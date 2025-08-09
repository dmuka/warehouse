using AspNetCore.Swagger.Themes;

namespace Warehouse.Presentation.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSwaggerWithUi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(ModernStyle.Dark, options => options.EnableAllAdvancedOptions());

        return app;
    }
}
