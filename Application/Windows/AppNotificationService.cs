using Microsoft.Toolkit.Uwp.Notifications;

namespace Partidoro.Application.Windows
{
    public static class AppNotificationService
    {
        public static void Show(string text)
        {
            new ToastContentBuilder()
                .AddArgument("action, open")
                .AddText(text)
                .AddButton(new ToastButton()
                    .SetContent("Dismiss")
                    .AddArgument("action", "dismiss")
                    .SetBackgroundActivation())
                .Show();
        }
    }
}
