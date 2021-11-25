using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Api.Models;
using Api.Services;
using Microsoft.Extensions.Options;

namespace Api
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
            services.Configure<ChatDataBaseSettings>(
                Configuration.GetSection(nameof(ChatDataBaseSettings)));

            services.AddSingleton<IChatDataBaseSettings>(sp =>
                sp.GetRequiredService<IOptions<ChatDataBaseSettings>>().Value);
            services.AddSingleton<UserService>();
            services.AddSingleton<ContactosService>();
            services.AddSingleton<MensajesService>();
            services.AddSingleton<GruposService>();
            services.AddSingleton<MensajesGrupoService>();
            services.AddControllers();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
