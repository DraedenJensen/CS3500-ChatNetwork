/// <summary>
/// Author:    Draeden Jensen
/// Date:      04-02-2023
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500, Draeden Jensen, and John Haraden - This work may not 
///            be copied for use in Academic Coursework.
///
/// I, Draeden Jensen certify that this code was written from scratch and
/// we did not copy it in part or whole from another source.  All 
/// references used in the completion of the assignments are cited 
/// in the README file.
///
/// File Contents:
/// Contains the C# code for configuring settings of this MAUI project. All of this code was
/// automatically generated upon creating the project, but we added the .Services code which 
/// allowed for an implementation of our custom logger.
/// </summary>
 
using FileLogger;
using Microsoft.Extensions.Logging;

namespace ChatServer
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .Services.AddLogging(configure =>
                {
                    configure.AddDebug();
                    configure.SetMinimumLevel(LogLevel.Debug);
                    configure.AddProvider(new CustomFileLogProvider());
                })
                .AddTransient<MainPage>();

            return builder.Build();
        }
    }
}