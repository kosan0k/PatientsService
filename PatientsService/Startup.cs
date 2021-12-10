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
                ConfigureOutputFormatters(options.OutputFormatters);
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
            inputFormatters.Clear();
            inputFormatters.Add(new PatientJsonInputFormatter(new FhirJsonParser()));
        }

        private void ConfigureOutputFormatters(FormatterCollection<IOutputFormatter> outputFormatters)
        {
            outputFormatters.Clear();
            outputFormatters.Add(new PatientJsonOutputFormatter());
        }
    }
}
