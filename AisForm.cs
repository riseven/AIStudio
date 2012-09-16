using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AIS
{
    public partial class AisForm : Form
    {
        private TreeView treeView;
        private DataGridView propiedadesDataGrid;

        private Proyecto proyecto;
        private AISObject aisObjectSelected;

        public AisForm()
        {
            InitializeComponent();

            // Creamos el contenido del explorer
            treeView = new TreeView();
            treeView.Dock = DockStyle.Fill ;
            treeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(treeView_NodeMouseClick);
            
            tabControlLeft.TabPages["tabPageExplorer"].Controls.Add(treeView);



            // Creamos el visor de propiedades
            propiedadesDataGrid = new DataGridView();

            propiedadesDataGrid.AllowUserToAddRows = false;
            propiedadesDataGrid.AllowUserToDeleteRows = false;
            propiedadesDataGrid.AllowUserToOrderColumns = false;
            propiedadesDataGrid.AllowUserToResizeColumns = true;
            propiedadesDataGrid.AllowUserToResizeRows = false;
            propiedadesDataGrid.BorderStyle = BorderStyle.FixedSingle;
            propiedadesDataGrid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            propiedadesDataGrid.ColumnHeadersVisible = false;
            propiedadesDataGrid.MultiSelect = false;
            propiedadesDataGrid.RowHeadersVisible = false;
            propiedadesDataGrid.Dock = DockStyle.Fill;
            
            propiedadesDataGrid.Columns.Add("key", "");
            propiedadesDataGrid.Columns.Add("value", "");
            propiedadesDataGrid.Columns["key"].ReadOnly = true;

            propiedadesDataGrid.CellValidating += new DataGridViewCellValidatingEventHandler(propiedadesDataGrid_CellValidating);

            tabControlLeft.TabPages["tabPagePropiedades"].Controls.Add(propiedadesDataGrid);

        }

        void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode node = e.Node;

            // Buscamos el nodo por navegacion
            List<TreeNode> navegacion = new List<TreeNode>();

            while (!treeView.Nodes.Contains(node))
            {
                navegacion.Add(node);
                node = node.Parent;
            }

            // Ahora recorremos la navegacion buscando el AISObject
            AISObject objeto = proyecto;
            while (navegacion.Count > 0)
            {
                foreach (AISObject c in objeto.Content)
                {
                    if (c.Name == navegacion[0].Text)
                    {
                        objeto = c;
                        navegacion.RemoveAt(0);
                        break;
                    }
                }
            }

            // Ahora marcamos objeto como seleccionado
            SeleccionarObjeto(objeto);
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // Creamos el proyecto
                FileInfo fileInfo = new FileInfo(sfd.FileName);
                proyecto = new Proyecto(fileInfo);

                // Actualizamos el treeview
                ActualizeTreeView();

                SeleccionarObjeto(proyecto);

                // Ahora tenemos proyecto abierto
                splitContainer.Visible = true;
            }
        }

        public void propiedadesDataGrid_CellValidating(Object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (propiedadesDataGrid.Columns[e.ColumnIndex].Name == "value")
            {
                string key = (string)propiedadesDataGrid.Rows[e.RowIndex].Cells["key"].Value;
                string value = (string)e.FormattedValue;
                aisObjectSelected.SetPropiedad(key, value);

                ActualizeTreeView();
            }
        }

        private void generarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            proyecto.GenerateProject();
        }

        private void addItemToProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddItemToProjectForm aitpf = new AddItemToProjectForm();
            aitpf.ObjectName = "UserObject";

            List<AISObject> aisObjects = new List<AISObject>();
            aisObjects.Add(new GenericAIObject("Agent", "AI::"));
            aisObjects.Add(new GenericAIObject("AIObject", "AI::"));
            aisObjects.Add(new GenericAIObject("Group", "AI::"));
            aisObjects.Add(new GenericAIObject("SelectiveGroup", "AI::"));
            aisObjects.Add(new GenericAIObject("Clock", "AI::"));
            aisObjects.Add(new GenericAIObject("AIObject", "AI::"));
            

            foreach (AISObject o in aisObjects)
            {
                aitpf.BaseObjects.Add(o.Name);
            }

            if (aitpf.ShowDialog() == DialogResult.OK)
            {
                // Buscamos el base object
                foreach (AISObject o in aisObjects)
                {
                    if (o.Name == (string)aitpf.SelectedObject)
                    {
                        proyecto.Content.Add(new UserObject(aitpf.ObjectName, o));
                        break;
                    }
                }

                ActualizeTreeView();
            }
        }

        private void ActualizeTreeView()
        {
            treeView.Nodes.Clear();
            treeView.Nodes.Add(proyecto.GetTreeNode());
        }

        private void SeleccionarObjeto(AISObject o)
        {
            // Seleccionamos el proyecto
            aisObjectSelected = o;

            // Actualizamos la lista de propiedades
            o.ActualizarPropiedades(propiedadesDataGrid);
        }
    }
}