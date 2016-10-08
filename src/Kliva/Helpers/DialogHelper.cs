using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Kliva.Helpers
{
    public static class DialogHelper
    {
        public static Task<bool> AskUserAsync(string title, string message, string yesText, string noText)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            AskUser(title, message, yesText, noText, () => tcs.SetResult(true), () => tcs.SetResult(false));

            return tcs.Task;
        }

        public static void AskUser(string title, string message, string yesText, string noText,
            Action actionOnPositive)
        {
            AskUser(title, message, yesText, noText, actionOnPositive, null);
        }

        public static void AskUser(string title, string message, string yesText, string noText,
            Action actionOnPositive, Action actionOnNegative)
        {
            MessageDialog msg = new MessageDialog(message, title);

            msg.Commands.Add(new UICommand(
                yesText, cmd =>
                {
                    actionOnPositive.Invoke();
                }));

            if (actionOnNegative == null)
            {
                msg.Commands.Add(new UICommand(noText));
            }
            else
            {
                msg.Commands.Add(new UICommand(noText, cmd =>
                {
                    actionOnNegative.Invoke();
                }));
            }

            msg.CancelCommandIndex = 1;

            msg.ShowAsync().AsTask();
        }

        public static async Task TellUserAsync(string title, string message, string buttonText)
        {
            MessageDialog msg = new MessageDialog(message, title);
            msg.Commands.Add(new UICommand
            {
                Label = buttonText
            });
            await msg.ShowAsync();
        }
    }
}
