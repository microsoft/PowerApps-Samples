namespace MetaViz
{
    partial class frmGraphViewer
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
            this.panelL = new System.Windows.Forms.Panel();
            this.lvEntity = new System.Windows.Forms.ListView();
            this.entityName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelLT = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.diagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openERJsonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawSelectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.drawRelatedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoDiagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.triggerInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.entityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tickRelatedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeEntityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyURLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.hideEntityViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelM = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.openJsonDialog = new System.Windows.Forms.OpenFileDialog();
            this.panelL.SuspendLayout();
            this.panelLT.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panelM.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelL
            // 
            this.panelL.Controls.Add(this.lvEntity);
            this.panelL.Controls.Add(this.panelLT);
            this.panelL.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelL.Location = new System.Drawing.Point(0, 0);
            this.panelL.Name = "panelL";
            this.panelL.Size = new System.Drawing.Size(230, 450);
            this.panelL.TabIndex = 0;
            // 
            // lvEntity
            // 
            this.lvEntity.CheckBoxes = true;
            this.lvEntity.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.entityName});
            this.lvEntity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvEntity.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvEntity.HideSelection = false;
            this.lvEntity.Location = new System.Drawing.Point(0, 27);
            this.lvEntity.Name = "lvEntity";
            this.lvEntity.Size = new System.Drawing.Size(230, 423);
            this.lvEntity.TabIndex = 7;
            this.lvEntity.UseCompatibleStateImageBehavior = false;
            this.lvEntity.View = System.Windows.Forms.View.Details;
            // 
            // panelLT
            // 
            this.panelLT.Controls.Add(this.menuStrip1);
            this.panelLT.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLT.Location = new System.Drawing.Point(0, 0);
            this.panelLT.Name = "panelLT";
            this.panelLT.Size = new System.Drawing.Size(230, 27);
            this.panelLT.TabIndex = 2;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.diagramToolStripMenuItem,
            this.selectionToolStripMenuItem,
            this.entityToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(230, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // diagramToolStripMenuItem
            // 
            this.diagramToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openERJsonToolStripMenuItem,
            this.drawSelectedToolStripMenuItem,
            this.drawRelatedToolStripMenuItem,
            this.undoDiagramToolStripMenuItem,
            this.triggerInformationToolStripMenuItem});
            this.diagramToolStripMenuItem.Name = "diagramToolStripMenuItem";
            this.diagramToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.diagramToolStripMenuItem.Text = "Diagram";
            // 
            // openERJsonToolStripMenuItem
            // 
            this.openERJsonToolStripMenuItem.Name = "openERJsonToolStripMenuItem";
            this.openERJsonToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.openERJsonToolStripMenuItem.Text = "Open ER Json File";
            this.openERJsonToolStripMenuItem.Click += new System.EventHandler(this.openERJsonToolStripMenuItem_Click);
            // 
            // drawSelectedToolStripMenuItem
            // 
            this.drawSelectedToolStripMenuItem.Name = "drawSelectedToolStripMenuItem";
            this.drawSelectedToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.drawSelectedToolStripMenuItem.Text = "Draw Selected Entities";
            this.drawSelectedToolStripMenuItem.Click += new System.EventHandler(this.drawSelectedToolStripMenuItem_Click);
            // 
            // drawRelatedToolStripMenuItem
            // 
            this.drawRelatedToolStripMenuItem.Name = "drawRelatedToolStripMenuItem";
            this.drawRelatedToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.drawRelatedToolStripMenuItem.Text = "Draw Related Entities";
            this.drawRelatedToolStripMenuItem.Click += new System.EventHandler(this.drawRelatedToolStripMenuItem_Click);
            // 
            // undoDiagramToolStripMenuItem
            // 
            this.undoDiagramToolStripMenuItem.Name = "undoDiagramToolStripMenuItem";
            this.undoDiagramToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.undoDiagramToolStripMenuItem.Text = "Undo Diagram";
            this.undoDiagramToolStripMenuItem.Click += new System.EventHandler(this.undoDiagramToolStripMenuItem_Click);
            // 
            // triggerInformationToolStripMenuItem
            // 
            this.triggerInformationToolStripMenuItem.Name = "triggerInformationToolStripMenuItem";
            this.triggerInformationToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.triggerInformationToolStripMenuItem.Text = "Trigger Information";
            this.triggerInformationToolStripMenuItem.Click += new System.EventHandler(this.triggerInformationToolStripMenuItem_Click);
            // 
            // selectionToolStripMenuItem
            // 
            this.selectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem});
            this.selectionToolStripMenuItem.Name = "selectionToolStripMenuItem";
            this.selectionToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.selectionToolStripMenuItem.Text = "Entities";
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // entityToolStripMenuItem
            // 
            this.entityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tickRelatedToolStripMenuItem,
            this.removeEntityToolStripMenuItem,
            this.copyURLToolStripMenuItem,
            this.toolStripSeparator1,
            this.hideEntityViewerToolStripMenuItem});
            this.entityToolStripMenuItem.Name = "entityToolStripMenuItem";
            this.entityToolStripMenuItem.Size = new System.Drawing.Size(96, 20);
            this.entityToolStripMenuItem.Text = "Selected Entity";
            // 
            // tickRelatedToolStripMenuItem
            // 
            this.tickRelatedToolStripMenuItem.Name = "tickRelatedToolStripMenuItem";
            this.tickRelatedToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.tickRelatedToolStripMenuItem.Text = "Select Related Entities";
            this.tickRelatedToolStripMenuItem.Click += new System.EventHandler(this.tickRelatedToolStripMenuItem_Click);
            // 
            // removeEntityToolStripMenuItem
            // 
            this.removeEntityToolStripMenuItem.Name = "removeEntityToolStripMenuItem";
            this.removeEntityToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.removeEntityToolStripMenuItem.Text = "Remove Entity";
            this.removeEntityToolStripMenuItem.Click += new System.EventHandler(this.removeEntityToolStripMenuItem_Click);
            // 
            // copyURLToolStripMenuItem
            // 
            this.copyURLToolStripMenuItem.Name = "copyURLToolStripMenuItem";
            this.copyURLToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.copyURLToolStripMenuItem.Text = "Copy URL";
            this.copyURLToolStripMenuItem.Click += new System.EventHandler(this.copyURLToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(185, 6);
            // 
            // hideEntityViewerToolStripMenuItem
            // 
            this.hideEntityViewerToolStripMenuItem.Name = "hideEntityViewerToolStripMenuItem";
            this.hideEntityViewerToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.hideEntityViewerToolStripMenuItem.Text = "Hide Scheme Viewer";
            this.hideEntityViewerToolStripMenuItem.Click += new System.EventHandler(this.hideEntityViewerToolStripMenuItem_Click);
            // 
            // panelM
            // 
            this.panelM.Controls.Add(this.statusStrip1);
            this.panelM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelM.Location = new System.Drawing.Point(230, 0);
            this.panelM.Name = "panelM";
            this.panelM.Size = new System.Drawing.Size(570, 450);
            this.panelM.TabIndex = 1;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(570, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(555, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // openJsonDialog
            // 
            this.openJsonDialog.Filter = "Json files|*.json";
            this.openJsonDialog.Title = "Open metadata visualizer json file";
            // 
            // frmGraphViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panelM);
            this.Controls.Add(this.panelL);
            this.Name = "frmGraphViewer";
            this.Text = "ER Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmGraphViewer_FormClosing);
            this.Load += new System.EventHandler(this.frmGraphViewer_Load);
            this.panelL.ResumeLayout(false);
            this.panelLT.ResumeLayout(false);
            this.panelLT.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelM.ResumeLayout(false);
            this.panelM.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelL;
        private System.Windows.Forms.Panel panelM;
        private System.Windows.Forms.Panel panelLT;
        private System.Windows.Forms.OpenFileDialog openJsonDialog;
        private System.Windows.Forms.ListView lvEntity;
        private System.Windows.Forms.ColumnHeader entityName;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem diagramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openERJsonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem entityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drawSelectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem drawRelatedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoDiagramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyURLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tickRelatedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeEntityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideEntityViewerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem selectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem triggerInformationToolStripMenuItem;
    }
}