// =====================================================================
//  This file is part of the Microsoft Dynamics CRM SDK code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.

//  This sample is created using the CRM SDK Template (WPF Application for CRM)
//  that provides a common login control (LoginWindow\CrmLogin.xaml)and 
//  built in authentication and credential-caching support.
//  For more information, see http://go.microsoft.com/fwlink/?LinkId=521516.

//<snippetCRUDOperationsusingXRMTooling>
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

// This namespace is automatically added for using 
// components in LoginWindow\CrmLogin.xaml (common login control).
using QuickStartXRMToolingWPFClient.LoginWindow;

// Add this namespace for performing 
// various operations in CRM.
using Microsoft.Xrm.Tooling.Connector;

namespace QuickStartXRMToolingWPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Demonstrates how to do basic entity operations like create, retrieve,
        /// update, and delete using the XRM Tooling APIs and the common login
        /// control.</summary>
        /// <remarks>
        /// At run-time, you will be given the option to delete all the
        /// database records created by this program.</remarks>
        public MainWindow()
        {
            InitializeComponent();
            btnDelete.Visibility = Visibility.Hidden; // Hide the Delete button when the form loads.
        }

        #region Class Level Members

        private CrmLogin _ctrl = null;
        private Guid _accountId;

        #endregion Class Level Members

        #region How To Sample Code

        /// <summary>
        /// Connect to Microsoft Dynamics CRM, and get an instance of CRMServiceClient 
        /// </summary>

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the XRM Tooling common login control
            _ctrl = new CrmLogin();

            /// Wire event to the CRM sign-in response. 
            _ctrl.ConnectionToCrmCompleted += ctrl_ConnectionToCrmCompleted;
            UpdateStatus("Initiating connection to CRM...");

            /// Show the XRM Tooling common login control. 
            _ctrl.ShowDialog();

            /// Validate if you are connected to CRM 
            if (_ctrl.CrmConnectionMgr != null && _ctrl.CrmConnectionMgr.CrmSvc != null && _ctrl.CrmConnectionMgr.CrmSvc.IsReady)
            {
                UpdateStatus("Connected to CRM! (Version: " + _ctrl.CrmConnectionMgr.CrmSvc.ConnectedOrgVersion.ToString() +
                    "; Org: " + _ctrl.CrmConnectionMgr.CrmSvc.ConnectedOrgUniqueName.ToString() + ")");
                UpdateStatus("***************************************");
                UpdateStatus("Click Start to create, retrieve, update, and delete (optional) an account record.");
                btnSignIn.IsEnabled = false;
                btnCRUD.IsEnabled = true;
            }
            // If you are not connected to CRM; display the last error and last CRM excption
            else
            {
                UpdateStatus("The connection to CRM failed or was cancelled by the user.");
            }
        }

        private void btnCRUD_Click(object sender, RoutedEventArgs e)
        {
            // Create an account record
            Dictionary<string, CrmDataTypeWrapper> inData = new Dictionary<string, CrmDataTypeWrapper>();
            inData.Add("name", new CrmDataTypeWrapper("Sample Account Name", CrmFieldType.String));
            inData.Add("address1_city", new CrmDataTypeWrapper("Redmond", CrmFieldType.String));
            inData.Add("telephone1", new CrmDataTypeWrapper("555-0160", CrmFieldType.String));
            _accountId = _ctrl.CrmConnectionMgr.CrmSvc.CreateNewRecord("account", inData);

            // Validate if the account is created, and then retrieve it
            if (_accountId != Guid.Empty)
            {
                UpdateStatus("***************************************");
                UpdateStatus(DateTime.Now.ToString() + ": Created an account with the following" + "\n" + "details, and retrieved it:");
                Dictionary<string, object> data = _ctrl.CrmConnectionMgr.CrmSvc.GetEntityDataById("account", _accountId, null);
                foreach (var pair in data)
                {
                    if ((pair.Key == "name") || (pair.Key == "address1_city") || (pair.Key == "telephone1"))
                    {
                        UpdateStatus(pair.Key.ToUpper() + ": " + pair.Value);
                    }
                }
                btnCRUD.IsEnabled = false;
            }
            else
            {
                UpdateStatus("***************************************");
                UpdateStatus("Could not create the account.");
            }


            // Update the account record
            Dictionary<string, CrmDataTypeWrapper> updateData = new Dictionary<string, CrmDataTypeWrapper>();
            updateData.Add("name", new CrmDataTypeWrapper("Updated Sample Account Name", CrmFieldType.String));
            updateData.Add("address1_city", new CrmDataTypeWrapper("Boston", CrmFieldType.String));
            updateData.Add("telephone1", new CrmDataTypeWrapper("555-0161", CrmFieldType.String));
            bool updateAccountStatus = _ctrl.CrmConnectionMgr.CrmSvc.UpdateEntity("account", "accountid", _accountId, updateData);

            // Validate if the the account record was updated successfully, and then display the updated information
            if (updateAccountStatus == true)
            {
                UpdateStatus("***************************************");
                UpdateStatus(DateTime.Now.ToString() + ": Updated the account details as follows:");
                Dictionary<string, object> data = _ctrl.CrmConnectionMgr.CrmSvc.GetEntityDataById("account", _accountId, null);
                foreach (var pair in data)
                {
                    if ((pair.Key == "name") || (pair.Key == "address1_city") || (pair.Key == "telephone1"))
                    {
                        UpdateStatus(pair.Key.ToUpper() + ": " + pair.Value);
                    }
                }
            }
            else
            {
                UpdateStatus("***************************************");
                UpdateStatus("Could not update the account.");
            }

            // Prompt the user to delete the account record created in the sample
            UpdateStatus("***************************************");
            UpdateStatus("To delete the account record created by the sample, click Delete Record.");
            UpdateStatus("Otherwise, click Exit to close the sample application.");
            btnDelete.Visibility = Visibility.Visible;
        }

        // Delete the account record created in this sample.
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            btnDelete.IsEnabled = false;
            _ctrl.CrmConnectionMgr.CrmSvc.DeleteEntity("account", _accountId);
            UpdateStatus("***************************************");
            UpdateStatus(DateTime.Now.ToString() + ": Deleted the account record.");
            UpdateStatus("Click Exit to close the sample application.");
        }

        // Exit the sample application
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Progressively displays the status messages about the actions
        /// performed during the running of the sample.
        /// <param name="updateText">Indicates the status string that needs to be 
        /// displayed to the user.</param>
        /// </summary>
        public void UpdateStatus(string updateText)
        {
            if (lblStatus.Content.ToString() != String.Empty)
            {
                lblStatus.Content = lblStatus.Content + "\n" + updateText;
            }
            else
            {
                lblStatus.Content = updateText;
            }
        }

        /// <summary>
        /// Raised when the login process is completed.
        /// </summary>
        private void ctrl_ConnectionToCrmCompleted(object sender, EventArgs e)
        {
            if (sender is CrmLogin)
            {
                this.Dispatcher.Invoke(() =>
                {
                    ((CrmLogin)sender).Close();
                });
            }
        }
    }
    #endregion How To Sample Code
}
//</snippetCRUDOperationsusingXRMTooling>
