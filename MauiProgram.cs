using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using The49.Maui.BottomSheet;

namespace PlayField
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            FormHandler.RemoveBorders();
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiCommunityToolkit()
                .UseBottomSheet()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Raleway-Light.ttf", "Raleway Light");
                    fonts.AddFont("Raleway-Regular.ttf", "Raleway Regular");
                    fonts.AddFont("Raleway-Medium.ttf", "Raleway Medium");
                    fonts.AddFont("Raleway-Bold.ttf", "Raleway Bold");
                    fonts.AddFont("Raleway-Black.ttf", "Raleway Black");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
