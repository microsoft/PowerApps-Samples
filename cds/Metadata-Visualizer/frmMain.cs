using System;
using System.Windows.Forms;
using System.IO;
using System.Configuration;

namespace MetaViz
{
    public partial class frmMain : Form
    {
        private const string DOWNLOAD = "Download Metadata";
        private const string CANCEL = "Cancel";

        private const string SAMPLE_URL_ONLINE = "https://contoso.crm.dynamics.com";
        private const string DEFAULT_STATUS = "Please enter the organization url then push Download Metadata.";

        private MetadataDownloader metadataDownloader = null;   // controller to initiate the download metadata and parse the response
        private frmGraphViewer graphViewer = null;              // ER viewer form

        public frmMain()
        {
            InitializeComponent();
            txtBaseUrl.Text = SAMPLE_URL_ONLINE;
            labelStatus.Text = DEFAULT_STATUS;
            // if you only need the trigger information html, you can make this check box visible and untick to avoid scheme download
            cbFullSpec.Visible = false; 
        }

        private void DownloadMetadata()
        {
            if (!CheckUrlTextField()) return;
            if (string.IsNullOrWhiteSpace(folderBrowserDialog1.SelectedPath) || !Directory.Exists(folderBrowserDialog1.SelectedPath))
            {
                if (DialogResult.OK != folderBrowserDialog1.ShowDialog()) return;
            }
            if (!Directory.Exists(folderBrowserDialog1.SelectedPath))
            {
                labelStatus.Text = $"{folderBrowserDialog1.SelectedPath} does not exist.";
                return;
            }

            btnGraph.Enabled = false;
            btnDumpMetadata.Text = CANCEL;
            if (graphViewer != null) graphViewer.Close();

            // Now start to run the spec dump procedures.
            metadataDownloader = new MetadataDownloader(txtBaseUrl.Text, folderBrowserDialog1.SelectedPath, cbFullSpec.Checked);

            string endpointVersion = ConfigurationManager.AppSettings["ENDPOINT_VERSION"];
            if (!string.IsNullOrWhiteSpace(endpointVersion)) metadataDownloader.EndpointVersion = endpointVersion;

            // use the adal library
            StartDownloadMetadataUsingAdalAndBackgroundWorker();
        }

        private void StartDownloadMetadataUsingAdalAndBackgroundWorker()
        {
            if (adalDownloadBGworker.IsBusy) return;
            adalDownloadBGworker.WorkerReportsProgress = true;
            adalDownloadBGworker.RunWorkerAsync();
        }

        private void adalDownloadBGworker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                DownloadMetadataUsingAdal();
            }
            catch (Exception ex)
            {
                metadataDownloader.ExceptionCaught = ex;
            }
        }

        private void DownloadMetadataUsingAdal()
        {
            adalDownloadBGworker.ReportProgress(0);
            HttpDownloadClient httpDownloadClient = new HttpDownloadClient(txtBaseUrl.Text);
            httpDownloadClient.Connect(metadataDownloader.WHOAMIURL);
            string nextWebApiUrl = metadataDownloader.GetNextWebApiUrl();
            while (nextWebApiUrl != null && !metadataDownloader.Cancelled)
            {
                string statusText = $"Downloading {new Uri(nextWebApiUrl).AbsolutePath}";
                adalDownloadBGworker.ReportProgress(50, statusText);
                string content = httpDownloadClient.Fetch(nextWebApiUrl);
                metadataDownloader.HandleResponse(content);
                nextWebApiUrl = metadataDownloader.GetNextWebApiUrl();
            }
        }

        private void adalDownloadBGworker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
            {
                labelStatus.Text = "Testing connection and whoami";
            }
            else
            {
                string entityDownloadProgress = metadataDownloader.FetchAllEntitiesProgressText();
                labelStatus.Text = (string.IsNullOrWhiteSpace(entityDownloadProgress) ? e.UserState as string : entityDownloadProgress);
            }          
        }

        private void adalDownloadBGworker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            DownloadCompleted();
        }

        private void DownloadCompleted()
        {
            if (metadataDownloader.ExceptionCaught != null)
            {
                labelStatus.Text = "Exception caught during download.";
                MessageBox.Show(metadataDownloader.ExceptionCaught.ToString());
                Cleanup();
                return;
            }

            if (metadataDownloader.Cancelled)
            {
                labelStatus.Text = "Download cancelled.";
                Cleanup();
                return;
            }

            ShowReport();
            Cleanup();
        }

        private void ShowReport()
        {
            labelStatus.Text = $"Download completed. Output {metadataDownloader.OutputFolderPath}";

            string jsonFile = $"{metadataDownloader.OutputFileFullPath}.json";
            if (!File.Exists(jsonFile))
            {
                return;
            }

            graphViewer = new frmGraphViewer();
            graphViewer.LoadMetadataJsonFile(jsonFile);
            graphViewer.Show();
            graphViewer.FormClosing += GraphViewer_FormClosing;
        }

        private void Cleanup()
        {
            metadataDownloader = null;
            btnDumpMetadata.Text = DOWNLOAD;
            SetButtonsEnabled();
        }

        private void btnDumpMetadata_Click(object sender, EventArgs e)
        {
            if (CANCEL.Equals(btnDumpMetadata.Text) && metadataDownloader != null)
            {
                btnDumpMetadata.Text = "Cancelling";
                metadataDownloader.Cancelled = true;
                return;
            }
            if (SAMPLE_URL_ONLINE.Equals(txtBaseUrl.Text))
            {
                labelStatus.Text = "Please enter your organization URL";
                return;
            }

            DownloadMetadata();
        }

        private void btnGraph_Click(object sender, EventArgs e)
        {
            // show the empty graph viewer so that user can load graph file directly
            if (graphViewer != null) return;

            DialogResult result = openJsonDialog.ShowDialog();
            if (result != DialogResult.OK) return;

            string jsonFile = openJsonDialog.FileName;
            labelStatus.Text = $"Loading {Path.GetFileName(jsonFile)}.";
            graphViewer = new frmGraphViewer();
            graphViewer.LoadMetadataJsonFile(jsonFile);
            graphViewer.Show();
            graphViewer.FormClosing += GraphViewer_FormClosing;
        }

        private void GraphViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            graphViewer = null;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (graphViewer == null) return;
            graphViewer.Close();
        }

        private void SetButtonsEnabled()
        {
            btnDumpMetadata.Enabled = true;
            btnGraph.Enabled = true;
            if (!SAMPLE_URL_ONLINE.Equals(txtBaseUrl.Text)) return;
            txtBaseUrl.Text = SAMPLE_URL_ONLINE;
        }

        private bool CheckUrlTextField()
        {
            string organizationUrl = txtBaseUrl.Text.Trim();
            bool urlWellFormedAndHttp = Uri.IsWellFormedUriString(organizationUrl, UriKind.Absolute) && Uri.TryCreate(organizationUrl, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (!urlWellFormedAndHttp)
            {
                labelStatus.Text = DEFAULT_STATUS;
                return false;
            }
            if (organizationUrl.EndsWith("/")) organizationUrl = organizationUrl.Substring(0, organizationUrl.Length - 1);
            txtBaseUrl.Text = organizationUrl;
            return true;
        }

    }
}
