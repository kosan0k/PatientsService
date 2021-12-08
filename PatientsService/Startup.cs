using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PatientsService.Serialization.Formatters;

namespace PatientsService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                ConfigureInputFormatters(options.InputFormatters);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureInputFormatters(FormatterCollection<IInputFormatter> inputFormatters)
        {
            inputFormatters.RemoveType(typeof(SystemTextJsonInputFormatter));
            inputFormatters.Insert(0, new PatientJsonInputFormatter(new FhirJsonParser()));
        }
    }
}
