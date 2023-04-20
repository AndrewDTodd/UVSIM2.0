using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using UraniumUI;

using UVSim.ViewModel;
using UVSim;

namespace UVSim_View
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .RegisterModelElements()
                .RegisterViewModels()
                .RegisterViews();

#if DEBUG
		builder.Logging.AddDebug();
#endif
            return builder.Build();
        }

        public static MauiAppBuilder RegisterModelElements(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<ArchitectureSim_Interface<Int16>, BasicMLSim>();

            return builder;
        }

        public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<ArchitectureSimViewModel>();

            return builder;
        }

        public static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<EditorPage>();
            builder.Services.AddSingleton<SimPage>();

            return builder;
        }
    }
}