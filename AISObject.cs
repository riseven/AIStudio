using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace AIS
{
    abstract class AISObject
    {
        private string name;
        private List<AISObject> content = new List<AISObject>();

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public List<AISObject> Content
        {
            get
            {
                return content;
            }
        }

        public virtual string GetPropiedad(string key)
        {
            switch (key)
            {
                case "Name":
                    return Name;
            }
            throw new Exception("Propiedad no existe");
        }

        public virtual void SetPropiedad(string key, string valor)
        {
            switch (key)
            {
                case "Name":
                    Name = valor;
                    break;
            }
        }

        public virtual List<string> GetNombresPropiedades()
        {
            List<string> lista = new List<string>();

            lista.Add("Name");

            return lista;
        }

        public virtual TreeNode GetTreeNode()
        {
            TreeNode node = new TreeNode();

            foreach (AISObject o in Content)
            {
                node.Nodes.Add(o.GetTreeNode());
            }
            return node;
        }



        public void ActualizarPropiedades(DataGridView dataGrid)
        {
            dataGrid.Rows.Clear();

            List<string> nombres = this.GetNombresPropiedades();

            foreach (string s in nombres)
            {
                dataGrid.Rows.Add(s, this.GetPropiedad(s));
            }
        }

        public virtual string GetNameSpaceOpenCode()
        {
            return ""; 
        }
        public virtual string GetNameSpaceCloseCode()
        {
            return "";
        }

        public virtual string GetInitializeCode()
        {
            return Name + "::Initialize();\n";
        }
        public virtual string GetFinalizeCode()
        {
            return Name + "::Finalize();\n";
        }
        public virtual string GetIncludeCode()
        {
            return "#include \"" + Name + ".h\"\n";
        }
        public virtual string GetTypeCode()
        {
            return Name;
        }
        public abstract void GenerateOutputFiles(Proyecto project);


        protected void AddText(FileStream fs, string texto)
        {
            byte[] info = new UTF8Encoding().GetBytes(texto);
            fs.Write(info, 0, info.Length);
        }
    }
}
