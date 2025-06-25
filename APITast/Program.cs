
using BLL.Interfaces;
using BLL.Repositories;
using DAL.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Entities.UserMangment;

namespace APITast
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            //dbcontext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //identity 
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // lock account setting
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1); // 1 hr
                options.Lockout.MaxFailedAccessAttempts = 5; // 5 tries 
                options.Lockout.AllowedForNewUsers = true;  // lock it if 5 times wrong password 

                // password setting )
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            //This line prevents a cycle during conversion to JSON

            builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });




            //repo 
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


            //for controllers but make sure you do have servives in repo
            builder.Services.AddScoped<IRecipeService, RecipeService>();
            builder.Services.AddScoped<IRatingService, RatingService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();




            //added by me
            builder.Services.AddAuthorization();// give preivillage it has  [Authorize]



            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting(); // this one By reading the URL that arrived at the server and determining which controller/action should respond to this request
            //Without it, requests cannot be routed and you cannot receive and handle API requests or web pages. 

            app.UseAuthentication();


            app.UseAuthorization();


            app.MapControllers();

            SeedRolesAndAdminUser(app.Services).GetAwaiter().GetResult(); // call it (for role and admin) 




            app.Run();
        }

        //role
        public static async Task SeedRolesAndAdminUser(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            
            string[] roles = new[] { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            //first admin 
            string adminEmail = "admin@example.com";
            string adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true, 
                     FullName = "Admin441",
                    Gender = "NotSpecified",
                    Country = "Unknown"
                };

                var createUserResult = await userManager.CreateAsync(user, adminPassword);
                if (createUserResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                   
                    throw new Exception("Failed to create the admin user during seeding.");
                }
            }
        }
    }
}