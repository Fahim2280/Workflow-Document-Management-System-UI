namespace Workflow_Document_Management_System_UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();          
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
                options.Cookie.HttpOnly = true; // Make session cookie HTTP only
                options.Cookie.IsEssential = true; // Make session cookie essential
                options.Cookie.SameSite = SameSiteMode.Lax; // Set SameSite policy
            });
            // Add HttpClient for API calls from MVC controllers
            builder.Services.AddHttpClient();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // In your API's Program.cs


            app.UseCors("AllowUI");
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
