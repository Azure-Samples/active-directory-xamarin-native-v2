using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace active_directory_xamarin_intune
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            MainPage = new MainPage();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Unhandled Exception {e.ExceptionObject?.ToString()}");
            System.Diagnostics.Debugger.Break();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
