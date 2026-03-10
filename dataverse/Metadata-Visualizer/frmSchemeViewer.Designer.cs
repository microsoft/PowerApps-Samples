namespace MetaViz
{
    partial class frmSchemeViewer
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
            this.splitContainerT = new System.Windows.Forms.SplitContainer();
            this.lvAttributes = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvRelations = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerT)).BeginInit();
            this.splitContainerT.Panel1.SuspendLayout();
            this.splitContainerT.Panel2.SuspendLayout();
            this.splitContainerT.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerT
            // 
            this.splitContainerT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerT.Location = new System.Drawing.Point(0, 0);
            this.splitContainerT.Name = "splitContainerT";
            // 
            // splitContainerT.Panel1
            // 
            this.splitContainerT.Panel1.Controls.Add(this.lvAttributes);
            // 
            // splitContainerT.Panel2
            // 
            this.splitContainerT.Panel2.Controls.Add(this.lvRelations);
            this.splitContainerT.Size = new System.Drawing.Size(784, 361);
            this.splitContainerT.SplitterDistance = 600;
            this.splitContainerT.TabIndex = 3;
            this.splitContainerT.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerT_SplitterMoved);
            // 
            // lvAttributes
            // 
            this.lvAttributes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.lvAttributes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvAttributes.GridLines = true;
            this.lvAttributes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvAttributes.HideSelection = false;
            this.lvAttributes.Location = new System.Drawing.Point(0, 0);
            this.lvAttributes.Name = "lvAttributes";
            this.lvAttributes.Size = new System.Drawing.Size(600, 361);
            this.lvAttributes.TabIndex = 3;
            this.lvAttributes.UseCompatibleStateImageBehavior = false;
            this.lvAttributes.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Attribute";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Type";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Description";
            // 
            // lvRelations
            // 
            this.lvRelations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvRelations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvRelations.GridLines = true;
            this.lvRelations.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvRelations.HideSelection = false;
            this.lvRelations.Location = new System.Drawing.Point(0, 0);
            this.lvRelations.Name = "lvRelations";
            this.lvRelations.Size = new System.Drawing.Size(180, 361);
            this.lvRelations.TabIndex = 1;
            this.lvRelations.UseCompatibleStateImageBehavior = false;
            this.lvRelations.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Entity";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Relation";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Entity";
            // 
            // frmSchemeViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 361);
            this.Controls.Add(this.splitContainerT);
            this.Name = "frmSchemeViewer";
            this.Text = "Scheme Viewer";
            this.Load += new System.EventHandler(this.frmEntity_Load);
            this.SizeChanged += new System.EventHandler(this.frmEntity_SizeChanged);
            this.splitContainerT.Panel1.ResumeLayout(false);
            this.splitContainerT.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerT)).EndInit();
            this.splitContainerT.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerT;
        private System.Windows.Forms.ListView lvAttributes;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ListView lvRelations;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}