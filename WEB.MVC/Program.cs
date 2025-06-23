using BLL.Interfaces;
using BLL.Repositories;
using DAL.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Entities.UserMangment;

namespace WEB.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // for DbContext with SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // for GenericRepository
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //servives under Repository 

            builder.Services.AddScoped<IRecipeService, RecipeService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IRatingService, RatingService>();



            //identity 

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // close 5 min
                options.Lockout.MaxFailedAccessAttempts = 5; //after 5 wrong login 
                options.Lockout.AllowedForNewUsers = true;
            });





            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddControllers(); // for API controllers


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
            app.UseStatusCodePages(); //for the error messages 

            app.UseAuthentication();


            app.UseAuthorization();
            app.MapControllers(); // to use API endpoints 


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
