using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVCWithBlazor.Data;
using MVCWithBlazor.Services;

namespace MVCWithBlazor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Auxiliar>(); // Added by me for Test purpose
            // @"Server=172.16.4.165\SQLEXPRESS;Database=Don_DashboardReports;User Id=user; Password=Calarasi81; MultipleActiveResultSets=true;")
            services.AddDbContext<ReportDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ReportDbContext>(); // Add Identity 
            // Set different options for Identity
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;

                options.Lockout.MaxFailedAccessAttempts = 3; // After 3 attempts lock account
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10); // for a period of time
            });
            services.ConfigureApplicationCookie(options => // Set authentification to redirect to login and accessdenied actions 
            {
                options.LoginPath = "/Identity/Signin";
                options.AccessDeniedPath = "/Identiy/AccessDenied";
            });
            services.AddRazorPages(); // Added for Razor Pages
            services.AddServerSideBlazor(); // Added for Blazor
            services.AddControllersWithViews(); // Add MVC
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization(); // Add Identity
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages(); // Razor pages
                endpoints.MapBlazorHub(); // Added for blazor
            });
        }
    }
}
