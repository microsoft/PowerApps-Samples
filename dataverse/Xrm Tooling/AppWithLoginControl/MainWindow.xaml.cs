using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PowerApps.Samples.LoginUX;

namespace Desktop_app_using_login_control
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Instantiate the login control.  
            ExampleLoginForm ctrl = new ExampleLoginForm();

            // Wire the button click event to the login response.   
            ctrl.ConnectionToCrmCompleted += ctrl_ConnectionToCrmCompleted;

            // Show the login control.   
            ctrl.ShowDialog();

            // Check that a web service connection is returned and the service is ready.     
            if (ctrl.CrmConnectionMgr != null && ctrl.CrmConnectionMgr.CrmSvc != null && ctrl.CrmConnectionMgr.CrmSvc.IsReady)
            {
                // Display the Dataverse version and connected environment name  
                MessageBox.Show("Connected to Dataverse version: " + ctrl.CrmConnectionMgr.CrmSvc.ConnectedOrgVersion.ToString() +
                    " Organization: " + ctrl.CrmConnectionMgr.CrmSvc.ConnectedOrgUniqueName, "Connection Status");
                // TODO Additional web service operations can be performed here
            }
            else
            {
                MessageBox.Show("Cannot connect; try again!", "Connection Status");
            }
        }

        private void ctrl_ConnectionToCrmCompleted(object sender, EventArgs e)
        {
            if (sender is ExampleLoginForm)
            {
                this.Dispatcher.Invoke(() =>
                {
                    ((ExampleLoginForm)sender).Close();
                });
            }
        }
    }
}
