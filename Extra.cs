using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace PlayField
{
    public class Extra
    {
        public static void ShowToast(string message)
        {
            string text = message;
            ToastDuration duration = ToastDuration.Long;
            double fontSize = 20;
            var toast = Toast.Make(text, duration, fontSize);
            toast.Show();
        }
    }
}
