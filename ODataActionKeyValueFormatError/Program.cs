using Dependo.Autofac;
using Extenso.AspNetCore.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using ODataActionKeyValueFormatError.Data.Entities;

namespace ODataActionKeyValueFormatError
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseServiceProviderFactory(new DependableAutofacServiceProviderFactory());

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            builder.Services.AddControllers()
                .AddOData((options, serviceProvider) =>
                {
                    options.Select().Expand().Filter().OrderBy().SetMaxTop(null).Count();

                    ODataModelBuilder builder = new ODataConventionModelBuilder();
                    builder.EntitySet<LocalizableString>("LocalizableStringApi");

                    var getComparitiveTableFunction = builder.EntityType<LocalizableString>().Collection.Function("GetComparitiveTable");
                    getComparitiveTableFunction.Parameter<string>("cultureCode");
                    getComparitiveTableFunction.Returns<IActionResult>();

                    options.AddRouteComponents($"odata", builder.GetEdmModel());
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            // Use odata route debug, /$odata
            app.UseODataRouteDebug();

            // If you want to use /$openapi, enable the middleware.
            //app.UseODataOpenApi();

            // Add OData /$query middleware
            app.UseODataQueryRequest();

            // Add the OData Batch middleware to support OData $Batch
            //app.UseODataBatching();

            app.MapControllers();

            app.Run();
        }
    }
}