using System;
using System.Windows;
using System.Windows.Threading;

namespace JC2MapViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ShowError(e.Exception);
            e.Handled = true;
            Shutdown(1);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowError(e.ExceptionObject);
        }

        private static void ShowError(object error)
        {
            Exception ex = error as Exception;
            string message = "An unexpected error occurred:\n\n";

            if (ex != null)
            {
                Exception root = ex.GetBaseException();
                message += root.GetType().Name + ": " + root.Message;

                if (root != ex)
                {
                    message += "\n\nContext:\n" + ex.Message;
                }
            }
            else
            {
                message += error ?? "Unknown error";
            }

            MessageBox.Show(message, "JC2MapViewer - Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}


