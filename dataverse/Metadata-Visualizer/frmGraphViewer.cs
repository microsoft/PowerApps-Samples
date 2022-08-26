using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Color = Microsoft.Msagl.Drawing.Color;

namespace MetaViz
{
    internal partial class frmGraphViewer : Form
    {
        private readonly ToolTip viewerToolTip = new ToolTip();

        private GViewer gviewer;
        private ERInformation entityRelations;
        private Stack<List<string>> stackEntitiesForUndo = new Stack<List<string>>();
        private frmSchemeViewer entityViewer;

        // highlighted object by mouse hover. different color is set through attribute.
        // when pointer moves out color got be reverted back by setting original attribute.
        private IViewerObject selectedObjectMouseHover;

        // clicked / focused node. entity menus work on this entity.
        private IViewerObject clickedNode;

        private bool entityViewerEnabled = true;

        private string triggerHtmlFilePath = null;

        internal frmGraphViewer()
        {
            InitializeComponent();
        }

        private void frmGraphViewer_Load(object sender, EventArgs e)
        {
            // create a viewer object
            gviewer = new GViewer();
            gviewer.AsyncLayout = true;

            // create a tool viewer
            viewerToolTip.Active = true;
            viewerToolTip.AutoPopDelay = 5000;
            viewerToolTip.InitialDelay = 500;
            viewerToolTip.ReshowDelay = 500;

            gviewer.ObjectUnderMouseCursorChanged += Gviewer_ObjectUnderMouseCursorChanged;
            gviewer.MouseClick += Gviewer_MouseClick;

            // show the graph provided by default
            RefreshListView();
            DrawDiagram(0);

            //associate the viewer with the form
            SuspendLayout();
            gviewer.Dock = System.Windows.Forms.DockStyle.Fill;

            panelM.Controls.Add(gviewer);
            ResumeLayout();
        }

        private void Gviewer_ObjectUnderMouseCursorChanged(object sender, ObjectUnderMouseCursorChangedEventArgs e)
        {
            RestorePreviouslyHoverObjectColor(e.OldObject);
            if (e.OldObject != null) gviewer.Invalidate(e.OldObject);

            if (gviewer.ObjectUnderMouseCursor == null)
            {
                gviewer.SetToolTip(viewerToolTip, "");
                return;
            }

            selectedObjectMouseHover = gviewer.ObjectUnderMouseCursor;
            DrawingObject currentDrawingObject = selectedObjectMouseHover.DrawingObject;
            string selectedObjetLabel = null;
            if (currentDrawingObject is Edge)
            {
                var edge = currentDrawingObject as Edge;
                edge.Attr.Color = Color.Blue;
                selectedObjetLabel = edge.Attr.Id;  // Attr.Id holds the relationship information 
            }
            else if (currentDrawingObject is Node)
            {
                var node = currentDrawingObject as Node;
                node.Attr.Color = Color.Green;
                string entityName = node.Attr.Id;
                selectedObjetLabel = entityRelations.GetEREntitieAttributesByEntityName(entityName)?.FirstOrDefault<EREntityAttribute>()?.Description;
            }
            else
            {
                return;
            }
            gviewer.SetToolTip(viewerToolTip, selectedObjetLabel);
            if (e.NewObject != null) gviewer.Invalidate(e.NewObject);
        }

        private void RestorePreviouslyHoverObjectColor(IViewerObject previousViewerObject)
        {
            if (previousViewerObject == null) return;
            DrawingObject previousObject = previousViewerObject.DrawingObject;
            if (previousObject is Edge)
            {
                var edge = previousObject as Edge;
                edge.Attr.Color = Color.Black;
            }
            else if (previousObject is Node)
            {
                var node = previousObject as Node;
                node.Attr.Color = Color.Black;
            }
        }

        private void Gviewer_MouseClick(object sender, MouseEventArgs e)
        {
            if (!(selectedObjectMouseHover?.DrawingObject is Node)) return;

            RestoreSelectedNodeColor();
            clickedNode = selectedObjectMouseHover;
            (clickedNode.DrawingObject as Node).Attr.Color = Color.Red;

            string entityName = (selectedObjectMouseHover.DrawingObject as Node).Attr.Id;
            IEnumerable<ERRelation> relations = entityRelations.GetERRelationsForSpecificEntity(entityName);
            IEnumerable<EREntityAttribute> attributes = entityRelations.GetEREntitieAttributesByEntityName(entityName);

            string description = attributes?.FirstOrDefault<EREntityAttribute>()?.Description;
            if (description == null) description = "no entity description (sample data)";
            StatusText = $"{entityName} - {description}";

            if (!entityViewerEnabled) return;
            PrepareEntityViewer();
            entityViewer.ShowEntity(entityName, relations, attributes);
        }

        private void PrepareEntityViewer()
        {
            if (entityViewer != null) return;
            entityViewer = new frmSchemeViewer(triggerHtmlFilePath);
            entityViewer.Show();
            entityViewer.FormClosing += EntityViewer_FormClosing;
        }

        private void RestoreSelectedNodeColor()
        {
            if (clickedNode == null) return;
            if (!(clickedNode.DrawingObject is Node)) throw new InvalidOperationException("selectedNode is not Node object.");
            (clickedNode.DrawingObject as Node).Attr.Color = Color.Black;
        }

        private string StatusText
        {
            set { toolStripStatusLabel1.Text = value; toolStripStatusLabel1.Invalidate(); }
            get { return toolStripStatusLabel1.Text; }
        }
        
        private void openERJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = openJsonDialog.ShowDialog();
            if (result != DialogResult.OK) return;
            string fileNamePath = openJsonDialog.FileName;
            LoadMetadataJsonFile(fileNamePath);
        }

        internal void LoadMetadataJsonFile(string jsonFilePath)
        {
            string jsonFileName = Path.GetFileName(jsonFilePath);
            StatusText = $"Loading {jsonFileName}";

            ERInformation entityRelations = null;
            try
            {
                entityRelations = ERInformationUtil.LoadRelationsFromDataFile(jsonFilePath);
            }
            catch (Exception ex)
            {
                StatusText = $"Failed to load {jsonFileName} - " + ex.ToString();
                return;
            }
            this.entityRelations = entityRelations;

            if (File.Exists(jsonFilePath.Replace(".json", ".htm")))
            {
                triggerHtmlFilePath = jsonFilePath.Replace(".json", ".htm");
            }

            RefreshListView();
            StatusText = $"{Path.GetFileName(jsonFileName)} loaded.";
        }

        private void drawSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const int SELECTED_ENTITIES_ONLY = 0;
            DrawDiagram(SELECTED_ENTITIES_ONLY);
        }

        private void drawRelatedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const int SELECTED_ENTITIES_AND_RELATED = 1;
            DrawDiagram(SELECTED_ENTITIES_AND_RELATED);
        }
        
        private void undoDiagramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (stackEntitiesForUndo.Count <= 1)
            {
                // you need minimum 2 set of entities. currently used, and previous.
                return;
            }
            stackEntitiesForUndo.Pop(); // discard current set of entities
            SetSelectedEntitiesToListView(stackEntitiesForUndo.Pop());  // use the previous set of entities when graph was refreshed
            DrawDiagram(0);    // use the current set but do not push 
        }
       
        private void DrawDiagram(int depth)
        {
            //layout the graph and draw it
            //ranking layout looks most useful rendering top to bottom
            //https://www.syncfusion.com/blogs/post/visualize-custom-graph-ms-graph-layout-engine.aspx

            // show the relationship graph
            List<string> lstEntities = GetSelectedEntitiesFromListView();
            Graph graph = entityRelations.ConvertToGraph(lstEntities, depth);
            graph.LayoutAlgorithmSettings = new Microsoft.Msagl.Layout.MDS.MdsLayoutSettings();
            gviewer.Graph = graph;

            // lstEntities is expanded based on depth, set those to the list again so that you can remove unnecessary entities etc.
            if (depth > 0)
            {
                SetSelectedEntitiesToListView(lstEntities);
            }
            stackEntitiesForUndo.Push(lstEntities);
        }

        private void copyURLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string entityName = GetSelectedEntityNameInDiagram();
            if (entityName == null) return;
            string entityUrl = $"{entityRelations.OrganizationUrl}/main.aspx?etn={entityName}&pagetype=entitylist";
            Clipboard.SetText(entityUrl);
            StatusText = $"{entityUrl} copied to clipboard";
        }

        private void tickRelatedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string entityName = GetSelectedEntityNameInDiagram();
            if (entityName == null) return;

            StringBuilder addedEntities = new StringBuilder(" ");
            List<string> relatedEntities = entityRelations.FindRelatedEntities(entityName);
            int count = 0;
            foreach (ListViewItem item in lvEntity.Items)
            {
                if (!relatedEntities.Contains(item.Text) || item.Checked) continue;
                item.Checked = true;
                addedEntities.Append(item.Text + " ");
                count++;
            }
            lvEntity.Refresh();
            StatusText = $"{count} selected -{addedEntities.ToString()}";
        }

        private void removeEntityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string entityName = GetSelectedEntityNameInDiagram();
            if (entityName == null) return;
            gviewer.Graph.RemoveNode(clickedNode.DrawingObject as Node);
            gviewer.RemoveNode(clickedNode as IViewerNode, false);
            gviewer.Refresh();
            foreach (ListViewItem item in lvEntity.Items)
            {
                if (item.Text.Equals(entityName)) item.Checked = false;
            }
            StatusText = $"{entityName} removed from the diagram and selection.";
        }

        private void hideEntityViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            entityViewerEnabled = !entityViewerEnabled;
            hideEntityViewerToolStripMenuItem.Text = (entityViewerEnabled ? "Hide" : "Show") + " Scheme Viewer";
            if (entityViewerEnabled || entityViewer == null) return;
            entityViewer.Close();
            entityViewer = null;
        }

        private string GetSelectedEntityNameInDiagram()
        {
            if (!(clickedNode?.DrawingObject is Node))
            {
                StatusText = "No entity selected in the diagram.";
                return null;
            }
            return (clickedNode.DrawingObject as Node).Attr.Id;
        }

        private void EntityViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            entityViewer = null;
        }

        private void frmGraphViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (entityViewer == null) return;
            entityViewer.Close();
        }

        private static ListViewItem GetListViewClickedItem(object sender, MouseEventArgs e)
        {
            var senderList = (ListView)sender;
            var clickedItem = senderList.HitTest(e.Location).Item;
            return clickedItem;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // copy the selected entities into the clip board
            StringBuilder sb = new StringBuilder();
            foreach (ListViewItem item in lvEntity.Items)
            {
                if (item.Checked) sb.Append($"{item.Text},");
            }
            if (sb.Length == 0)
            {
                StatusText = "No entity is selected.";
                return;
            }
            Clipboard.SetText(sb.ToString());
            StatusText = "Selection copied to clipboard.";
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string selectionText = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(selectionText)) return;
            selectionText = selectionText.Replace(" ", "");
            string[] selections = selectionText.Split(new char[] { ',' });
            foreach (ListViewItem item in lvEntity.Items)
            {
                bool selected = (selections.Where(x => item.Text.Equals(x, StringComparison.InvariantCultureIgnoreCase)).Count() > 0);
                item.Checked = selected;
            }
            StatusText = "Selection copied from clipboard.";
            lvEntity.Refresh();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvEntity.Items)
            {
                item.Checked = false;
            }
            lvEntity.Invalidate();
        }

        private void RefreshListView()
        {
            lvEntity.Items.Clear();
            lvEntity.Columns[0].Width = lvEntity.Width - SystemInformation.VerticalScrollBarWidth - 5;
            foreach (string entiyName in entityRelations.ListEntities())
            {
                ListViewItem item = new ListViewItem(entiyName);
                bool entityInSkipList = ERInformation.IsEntityInSkipList(entiyName);
                item.Checked = !entityInSkipList;
                lvEntity.Items.Add(item);
            }
        }

        private List<string> GetSelectedEntitiesFromListView()
        {
            List<string> lstEntities = new List<string>();
            foreach (ListViewItem item in lvEntity.Items)
            {
                if (item.Checked) lstEntities.Add(item.Text);
            }
            return lstEntities;
        }

        private void SetSelectedEntitiesToListView(List<string> entityList)
        {
            List<string> lstEntities = new List<string>();
            foreach (ListViewItem item in lvEntity.Items)
            {
                bool isItemDisplayed = entityList.Contains(item.Text);
                item.Checked = isItemDisplayed;
            }
            lvEntity.Invalidate();
        }

        private void triggerInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(triggerHtmlFilePath))
            {
                StatusText = $"{triggerHtmlFilePath} not found.";
                return;
            }
            StatusText = $"Loading {triggerHtmlFilePath}....";
            ProcessStartInfo processStartInfo = new ProcessStartInfo(triggerHtmlFilePath);
            Process.Start(processStartInfo);
        }
    }
}
