using inference.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML;

namespace inference
{
    public class Startup
    {
        
        private IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore()
                .AddJsonFormatters()
                .AddControllersAsServices();

            var predictionFn = new Predictor(new MLContext(), Configuration.GetValue<string>("onnx")).GetPredictor();
            
            services.AddScoped(_ => new PredictionsController(predictionFn));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) => app.UseMvcWithDefaultRoute();
    }
}