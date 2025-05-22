using findspot_backend.Data;
using findspot_backend.DbInitializer;
using findspot_backend.Models;
using findspot_backend.Repositories;
using findspot_backend.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace findspot_backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FindSpotDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("FindSpotDbConnectionString")));

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<FindSpotDbContext>().AddDefaultTokenProviders();

            builder.Services.AddAuthorization();

            builder.Services.AddScoped<IDbInitializer, DbInitializer.DbInitializer>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITouristObjectRepository, TouristObjectRepository>();
            builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
            builder.Services.AddScoped<IUserBlogPostRepository, UserBlogPostRepository>();
            builder.Services.AddScoped<IImageRepository, ImageRepository>();
            builder.Services.AddScoped<ITagRepository, TagRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            SeedDataBase();

            app.UseHttpsRedirection();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();

            void SeedDataBase()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                    dbInitializer.Initialize();
                }
            }
        }
    }
}
