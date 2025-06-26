using BLL.Interfaces;
using BLL.Repositories;
using DAL.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace TastBudRecipesMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHttpClient(); //added by me 



            //        //dbcontext
            //        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //        //repo 
            //        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            //        //for controllers but make sure you do have servives in repo
            //        builder.Services.AddScoped<IRecipeService, RecipeService>();
            //        builder.Services.AddScoped<IRatingService, RatingService>();
            //        builder.Services.AddScoped<ICategoryService, CategoryService>();



            // Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";    // no register here 
                    options.LogoutPath = "/Account/Logout";  //logout 
                    options.Cookie.Name = "TasteBudAuthCookie"; // cookie name 
                });


            // i used it to get  cookies to get spesifc user 

            builder.Services.AddHttpContextAccessor(); 

            builder.Services.AddHttpClient("AuthenticatedClient")
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        UseCookies = false 
                    };
                });







            builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson();//added by me json request of body 


            builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    }); //by me 


            // Add services to the container.
            builder.Services.AddControllersWithViews();

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
