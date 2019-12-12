using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using Client.Helpers;
using Client.ViewModels;

namespace Client
{
    class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();

            ConventionManager.AddElementConvention<PasswordBox>(
            PasswordBoxHelper.BoundPasswordProperty,
            "Password",
            "PasswordChanged");
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }
    }
}
