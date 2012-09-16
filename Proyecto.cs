using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace AIS
{
    class Proyecto : AISObject
    {
        private FileInfo projectFile;
        private DirectoryInfo outputDir;
        private string nameSpace;

        public FileInfo ProjectFile
        {
            get
            {
                return projectFile;
            }
            set
            {
                projectFile = value;
            }
        }
        public DirectoryInfo OutputDir
        {
            get
            {
                return outputDir;
            }
            set
            {
                outputDir = value;
            }
        }
        public string NameSpace
        {
            get
            {
                return nameSpace;
            }
            set
            {
                nameSpace = value;
            }
        }

        public Proyecto(FileInfo projectFile)
        {
            this.ProjectFile = projectFile ;
            this.Name = projectFile.Name.Substring(0, projectFile.Name.Length - projectFile.Extension.Length);
            this.OutputDir = projectFile.Directory;
            this.NameSpace = "";
        }



        public override List<string> GetNombresPropiedades()
        {
            List<string> lista = base.GetNombresPropiedades();
            lista.Add("ProjectFile");
            lista.Add("OutputDir");
            lista.Add("NameSpace");
            return lista;
        }

        public override string GetPropiedad(string key)
        {
            switch (key)
            {
                case "ProjectFile":
                    return ProjectFile.FullName;
                case "OutputDir":
                    return OutputDir.FullName;
                case "NameSpace":
                    return nameSpace;
                default:
                    return base.GetPropiedad(key);
            }
        }

        public override void SetPropiedad(string key, string valor)
        {
            switch (key)
            {
                case "ProjectFile":
                    ProjectFile = new FileInfo(valor);
                    break;
                case "OutputDir":
                    OutputDir = new DirectoryInfo(valor);
                    break;
                case "NameSpace":
                    NameSpace = valor;
                    break;
                default:
                    base.SetPropiedad(key, valor);
                    break;
            }
        }

        public override TreeNode GetTreeNode()
        {
            TreeNode node = base.GetTreeNode();
            node.Text = Name;

            return node;
        }

        public override string GetNameSpaceOpenCode()
        {
            if (NameSpace != "")
            {
                return "namespace " + NameSpace + " {\n";
            }
            else
            {
                return "";
            }
        }

        public override string GetNameSpaceCloseCode()
        {
            if (NameSpace != "")
            {
                return "}\n";
            }
            else
            {
                return "";
            }
        }

        public override void GenerateOutputFiles(Proyecto project)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void GenerateProject()
        {
            // Creamos el fichero .h
            GenerateProjectHeader();

            // Creamos el fichero .cpp
            GenerateProjectCpp();

            // Generamos los otros ficheros del proyecto
            foreach (AISObject o in Content)
            {
                o.GenerateOutputFiles(this);
            }
        }

        public virtual void GenerateProjectCpp()
        {
            FileStream cpp = File.Create(OutputDir.FullName + System.IO.Path.DirectorySeparatorChar + Name + ".cpp");

            string texto = "";

            // "#include "XXX.h"
            texto += "#include \"" + Name + ".h\"\n";
            texto += "\n";

            // namespace XXX {
            texto += GetNameSpaceOpenCode();
            texto += "\n";

            // bool XXX::initialized = false ;
            texto += "bool " + Name + "::initialized = false;\n";
            texto += "\n";

            // } ---namespace---
            texto += GetNameSpaceCloseCode();
            texto += "\n";

            AddText(cpp, texto);
            cpp.Close();
        }

        public void GenerateProjectHeader()
        {
            FileStream header = File.Create(OutputDir.FullName + System.IO.Path.DirectorySeparatorChar + Name + ".h");

            string texto = "" ;

            // #ifndef __XXX_H__
            // #define __XXX_H__
            texto += "#ifndef __" + Name + "_H__\n";
            texto += "#define __" + Name + "_H__\n";
            texto += "\n";

            // #include "AICommon.h"
            texto += "#include \"AICommon.h\"\n";
            // Seccion include dinamica
            foreach (AISObject o in Content)
            {
                texto += o.GetIncludeCode();
            }
            texto += "\n";

            // namespace XXX {
            texto += GetNameSpaceOpenCode();
            texto += "\n";

            // class XXX : public AI::MsgManager
            texto += "class " + Name + " {\n";

            // private:
            texto += "private:\n";

            //      static bool initialized;
            texto += "\tstatic bool initialized;\n";
            texto += "\n";

            // public:
            texto += "public:\n";
            
            //      static void Initialize() {
            //          if (initialized) {
            //              throw AI::Exception::Exception("Already initialized");
            //          }
            //
            //          AI::MsgManager::Initialize();
            //          AI::AIObject::Initialize();
            //      }
            texto += "\tstatic void Initialize() {\n";
            texto += "\t\tif (initialized) {\n";
            texto += "\t\t\tthrow AI::Exception::Exception(\"Already initialized\");\n";
            texto += "\t\t}\n";
            texto += "\t\tAI::MsgManager::Initialize();\n";
            texto += "\t\tAI::AIObject::Initialize();\n";
            // Seccion initialize dinamica
            foreach (AISObject o in Content)
            {
                string t = o.GetInitializeCode();
                if (t.Length > 0)
                {
                    texto += "\t\t" + t;
                }
            }
            texto += "\t}\n";
            texto += "\n";

            //      static void Finalize() {
            //          if (!initialized) {
            //              throw AI::Exception::Exception("Not initialized");
            //          }
            //
            //          AI::MsgManager::Finalize();
            //          AI::AIObject::Finalize();
            //      }
            texto += "\tstatic void Finalize() {\n";
            texto += "\t\tif (!initialized) {\n";
            texto += "\t\t\tthrow AI::Exception::Exception(\"Not initialized\");\n";
            texto += "\t\t}\n";
            texto += "\t\tAI::MsgManager::Finalize();\n";
            texto += "\t\tAI::AIObject::Finalize();\n";
            // Seccion finalize dinamica
            foreach (AISObject o in Content)
            {
                string t = o.GetFinalizeCode();
                if (t.Length > 0)
                {
                    texto += "\t\t" + t;
                }
            }
            texto += "\t}\n";
            texto += "\n";

            //      AI::MsgManager * GetManager() {
            //          return AI::MsgManager::GetManager() ;
            //      }
            texto += "\tAI::MsgManager * GetManager() {\n";
            texto += "\t\treturn AI::MsgManager::GetManager();\n" ;
            texto += "\t}\n";


            // } ---class---
            texto += "};\n";
            texto += "\n";

            // } ---namespace---
            texto += GetNameSpaceCloseCode();
            texto += "\n";

            // #endif
            texto += "#endif // __" + Name + "_H__\n";

            AddText(header, texto);

            header.Close();
        }
    }
}
