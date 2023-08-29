using LearningCards.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace LearningCards
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            builder.Services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                opts.LogTo(Console.WriteLine);
                opts.EnableSensitiveDataLogging();
                //opts.UseLazyLoadingProxies();
            }
            );

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opts =>
            {
                opts.Cookie.Name = "LearningCards.auth";

                //Bu o demekdir ki, istifadeci 7 gun boyuncu sign qalsin(?)
                opts.ExpireTimeSpan = TimeSpan.FromDays(7);

                //bunu true verende, istifadecinin sign in qalma muddeti loginden etibaren yox, her istifadeden sonra hesablanir
                // her gun istifade etse day bu sonsuza catar ki hahaha
                opts.SlidingExpiration = false;


                //Tutaq ki cookie-nin vaxti bitdi, sistem tapammadi, onda hara gedecek? yeniden logine.
                opts.LoginPath = "/Account/Login";
                //Cixis sehifesinin de pathini veririk
                opts.LogoutPath = "/Account/Logout";

                //Eger bu sehife bu cookie-li sexse qadagandirsa hara getsin?
                opts.AccessDeniedPath = "/Home/AccessDenied";


            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}