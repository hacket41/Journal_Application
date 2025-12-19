using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Journal.Data;
using Journal.Services;

namespace Journal
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            // Database path
            string dbPath = Path.Combine(
                FileSystem.AppDataDirectory,
                "journal.db"
            );

            // DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Filename={dbPath}")
            );

            // Journal service
            builder.Services.AddScoped<IJournalService, JournalService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // BUILD APP HERE
            var app = builder.Build();

            // CREATE DATABASE ON FIRST RUN
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }

            // RETURN APP
            return app;
        }
    }
}
