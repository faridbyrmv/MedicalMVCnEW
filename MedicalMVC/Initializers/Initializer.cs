using Medical.Domain.Entities;
using Medical.Mail;
using Medical.Persistence;
using Medical.Repositories.Repository;
using Medical.Repositories.Repository.Categoriesl;
using Medical.Repositories.Repository.ProductPhotos;
using Medical.Repositories.Repository.Products;
using Medical.Repositories.Repository.Requests;
using Medical.Repositories.Repository.Users;
using Medical.Service.Interfaces.Interfaces.Categories;
using Medical.Service.Interfaces.Interfaces.Products;
using Medical.Service.Interfaces.Interfaces.Requests;
using Medical.Service.Interfaces.Interfaces.Users;
using Medical.Services.Implementations.Implementations.Categories;
using Medical.Services.Implementations.Implementations.Products;
using Medical.Services.Implementations.Implementations.Requests;
using Medical.Services.Implementations.Implementations.Users;
using MedicalMVC.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MedicalMVC.Initializers;

public static class Initializer
{
    public static void Initializ(this IServiceCollection services, IConfiguration configuration)
    {
        //Authentication Settings
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Cookie.Name = "UserLoginCookie";
            options.LoginPath = "/account/login";
            options.LogoutPath = "/account/logout";
           options.ExpireTimeSpan = TimeSpan.FromHours(3);
            options.SlidingExpiration = true;
        });


        //Authorization Settings
        services.AddAuthorization();


        //SQL Connection Settings
        services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


        //Serilog settings
        Log.Logger = new LoggerConfiguration()
          .MinimumLevel.Information()
          .WriteTo.Console()
          .WriteTo.File("Logs/Mylogger.txt", rollingInterval: RollingInterval.Day)
          .CreateLogger();

        //Add Services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRequestService, RequestService>();
        services.AddScoped<IMailService, MailService>();


        services.AddScoped<IBaseRepository<ProductPhoto>, ProductPhotosRepository>();
        services.AddScoped<IBaseRepository<Product>, ProductRepository>();
        services.AddScoped<IBaseRepository<Category>, CategoryRepository>();
        services.AddScoped<IBaseRepository<User>, UserRepository>();
        services.AddScoped<IBaseRepository<Request>, RequestRepository>();

        services.AddScoped<ApplicationDbContext>();

    }
}
