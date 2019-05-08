using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Acr.UserDialogs;

namespace CentraMobile.Dialogs
{
    public static class AcrToast
    {
        public static void Success(string message, int seconds)
        {
            TimeSpan duration = new TimeSpan(0, 0, seconds);
            var tc = new ToastConfig(message);
            tc.BackgroundColor = Color.LawnGreen;
            tc.Icon = "success20x20.png";
            tc.MessageTextColor = Color.Black;
            tc.Duration = duration;
            //tc.Position = ToastPosition.Top;
            UserDialogs.Instance.Toast(tc);
        }

        public static void Error(string message, int seconds)
        {
            TimeSpan duration = new TimeSpan(0, 0, seconds);
            var tc = new ToastConfig(message);
            tc.BackgroundColor = Color.Red;
            tc.Icon = "error20x20.png";
            tc.MessageTextColor = Color.White;
            tc.Duration = duration;
            //tc.Position = ToastPosition.Top;
            UserDialogs.Instance.Toast(tc);
        }

        public static void Warning(string message, int seconds)
        {
            TimeSpan duration = new TimeSpan(0, 0, seconds);
            var tc = new ToastConfig(message);
            tc.BackgroundColor = Color.Yellow;
            tc.Icon = "warning20x20.png";
            tc.MessageTextColor = Color.Black;
            tc.Duration = duration;
            //tc.Position = ToastPosition.Top;
            UserDialogs.Instance.Toast(tc);
        }

        public static void Info(string message, int seconds)
        {
            TimeSpan duration = new TimeSpan(0, 0, seconds);
            var tc = new ToastConfig(message);
            tc.BackgroundColor = Color.DeepSkyBlue;
            tc.Icon = "added.png";
            tc.MessageTextColor = Color.White;
            tc.Duration = duration;
            //tc.Position = ToastPosition.Top;
            UserDialogs.Instance.Toast(tc);
        }
    }
}
