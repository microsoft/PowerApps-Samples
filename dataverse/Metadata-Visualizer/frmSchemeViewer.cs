using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MetaViz
{
    public partial class frmSchemeViewer : Form
    {
        private string triggerHtmlFilePath;

        public frmSchemeViewer(string triggerHtmlFilePath)
        {
            this.triggerHtmlFilePath = triggerHtmlFilePath;
            InitializeComponent();
        }

        internal void ShowEntity(string entityName, IEnumerable<ERRelation> relations, IEnumerable<EREntityAttribute> attributes)
        {
            this.Text = $"Scheme Viewer - {entityName}";
            ShowRelations(relations);
            ShowAttributes(attributes);
        }

        private void ShowRelations(IEnumerable<ERRelation> relations)
        {
            lvRelations.Items.Clear();
            foreach (ERRelation relation in relations)
            {
                ListViewItem item = null;
                if (relation.IsManyToMany())
                {
                    item = new ListViewItem(new string[] { relation.EntityOne, "n-n", relation.EntityMany });
                }
                else
                {
                    item = new ListViewItem(new string[] { relation.EntityOne, "1-n", $"{relation.EntityMany}.{relation.LookupField}" });
                }
                lvRelations.Items.Add(item);
            }
            lvRelations.Refresh();
        }

        private void ShowAttributes(IEnumerable<EREntityAttribute> attributes)
        {
            lvAttributes.Items.Clear();
            if (attributes.Count<EREntityAttribute>() == 0)
            {
                lvAttributes.Items.Add(new ListViewItem(new string[] { "Sample", "N/A", "Sample ER data does not have any attribute information." }));
                lvAttributes.Refresh();
                return;
            }
            foreach (EREntityAttribute attribute in attributes.OrderBy(def => def.AttributeName))
            {
                ListViewItem item = new ListViewItem(new string[] { attribute.AttributeName, attribute.DataType, attribute.Description });
                lvAttributes.Items.Add(item);
            }
            lvAttributes.Refresh();
        }

        private void ResizeListViewColumns()
        {
            int width = this.Width - SystemInformation.VerticalScrollBarWidth;
            int lvRelationColWidth = width / 8 * 3 / 8;
            lvRelations.Columns[0].Width = lvRelationColWidth * 3;
            lvRelations.Columns[1].Width = lvRelationColWidth;
            lvRelations.Columns[2].Width = lvRelations.Width - lvRelationColWidth * 4;

            int lvAttributeColWidth = width / 8 * 5 / 4;
            lvAttributes.Columns[0].Width = lvAttributeColWidth;
            lvAttributes.Columns[1].Width = lvAttributeColWidth;
            lvAttributes.Columns[2].Width = lvAttributes.Width - lvAttributeColWidth * 2;
        }

        private void frmEntity_Load(object sender, EventArgs e)
        {
            ResizeListViewColumns();
        }

        private void frmEntity_SizeChanged(object sender, EventArgs e)
        {
            ResizeListViewColumns();
        }

        private void splitContainerT_SplitterMoved(object sender, SplitterEventArgs e)
        {
            ResizeListViewColumns();
        }
    }
}
