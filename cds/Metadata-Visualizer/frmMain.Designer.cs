namespace MetaViz
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelT = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGraph = new System.Windows.Forms.Button();
            this.cbFullSpec = new System.Windows.Forms.CheckBox();
            this.txtBaseUrl = new System.Windows.Forms.TextBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.btnDumpMetadata = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.adalDownloadBGworker = new System.ComponentModel.BackgroundWorker();
            this.openJsonDialog = new System.Windows.Forms.OpenFileDialog();
            this.panelT.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelT
            // 
            this.panelT.Controls.Add(this.label1);
            this.panelT.Controls.Add(this.btnGraph);
            this.panelT.Controls.Add(this.cbFullSpec);
            this.panelT.Controls.Add(this.txtBaseUrl);
            this.panelT.Controls.Add(this.labelStatus);
            this.panelT.Controls.Add(this.btnDumpMetadata);
            this.panelT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelT.Location = new System.Drawing.Point(0, 0);
            this.panelT.Name = "panelT";
            this.panelT.Size = new System.Drawing.Size(464, 91);
            this.panelT.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "URL";
            // 
            // btnGraph
            // 
            this.btnGraph.Location = new System.Drawing.Point(316, 35);
            this.btnGraph.Name = "btnGraph";
            this.btnGraph.Size = new System.Drawing.Size(123, 23);
            this.btnGraph.TabIndex = 9;
            this.btnGraph.Text = "Open MetaViz Json";
            this.btnGraph.UseVisualStyleBackColor = true;
            this.btnGraph.Click += new System.EventHandler(this.btnGraph_Click);
            // 
            // cbFullSpec
            // 
            this.cbFullSpec.AutoSize = true;
            this.cbFullSpec.Checked = true;
            this.cbFullSpec.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbFullSpec.Location = new System.Drawing.Point(161, 39);
            this.cbFullSpec.Name = "cbFullSpec";
            this.cbFullSpec.Size = new System.Drawing.Size(73, 17);
            this.cbFullSpec.TabIndex = 7;
            this.cbFullSpec.Text = "All entities";
            this.cbFullSpec.UseVisualStyleBackColor = true;
            // 
            // txtBaseUrl
            // 
            this.txtBaseUrl.Location = new System.Drawing.Point(41, 9);
            this.txtBaseUrl.Name = "txtBaseUrl";
            this.txtBaseUrl.Size = new System.Drawing.Size(398, 20);
            this.txtBaseUrl.TabIndex = 3;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(6, 70);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(59, 13);
            this.labelStatus.TabIndex = 2;
            this.labelStatus.Text = "labelStatus";
            // 
            // btnDumpMetadata
            // 
            this.btnDumpMetadata.Location = new System.Drawing.Point(41, 35);
            this.btnDumpMetadata.Name = "btnDumpMetadata";
            this.btnDumpMetadata.Size = new System.Drawing.Size(114, 23);
            this.btnDumpMetadata.TabIndex = 1;
            this.btnDumpMetadata.Text = "Download Metadata";
            this.btnDumpMetadata.UseVisualStyleBackColor = true;
            this.btnDumpMetadata.Click += new System.EventHandler(this.btnDumpMetadata_Click);
            // 
            // adalDownloadBGworker
            // 
            this.adalDownloadBGworker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.adalDownloadBGworker_DoWork);
            this.adalDownloadBGworker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.adalDownloadBGworker_ProgressChanged);
            this.adalDownloadBGworker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.adalDownloadBGworker_RunWorkerCompleted);
            // 
            // openJsonDialog
            // 
            this.openJsonDialog.Filter = "Json files|*.json";
            this.openJsonDialog.Title = "Open metadata visualizer json file";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 91);
            this.Controls.Add(this.panelT);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "Metadata Visualizer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.panelT.ResumeLayout(false);
            this.panelT.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelT;
        private System.Windows.Forms.Button btnDumpMetadata;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.TextBox txtBaseUrl;
        private System.Windows.Forms.CheckBox cbFullSpec;
        private System.Windows.Forms.Button btnGraph;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.ComponentModel.BackgroundWorker adalDownloadBGworker;
        private System.Windows.Forms.OpenFileDialog openJsonDialog;
    }
}

