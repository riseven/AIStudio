using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AIS
{
    class UserObject : AISObject
    {
        private AISObject baseObject;

        public AISObject BaseObject
        {
            get
            {
                return baseObject;
            }
            set
            {
                baseObject = value;
            }
        }

        public UserObject(string name, AISObject baseObject)
        {
            Name = name;
            BaseObject = (baseObject != null) ? baseObject : this ;
        }

        public override TreeNode GetTreeNode()
        {
            TreeNode node = base.GetTreeNode();
            node.Text = Name;

            return node;
        }

        public override void GenerateOutputFiles(Proyecto project)
        {
            GenerateHeader(project);
            GenerateCpp(project);
        }

        public void GenerateHeader(Proyecto project)
        {
            // Generamos el fichero .h
            FileStream header = File.Create(project.OutputDir.FullName + System.IO.Path.DirectorySeparatorChar + Name + ".h");

            string texto = "";

            // #ifndef __XXX_H__
            // #define __XXX_H__
            texto += "#ifndef __" + Name + "_H__\n";
            texto += "#define __" + Name + "_H__\n";
            texto += "\n";

            // #include "AICommon.h"
            // #include "YYY.h" --- padre ---
            texto += "#include \"AICommon.h\"\n";
            texto += BaseObject.GetIncludeCode();
            texto += "\n";
            
            // #include <string>
            texto += "#include <string>\n";
            texto += "\n";

            // namespace ZZZ {
            texto += project.GetNameSpaceOpenCode();
            texto += "\n";

            // class XXX : public YYY
            texto += "class " + Name + " : public " + BaseObject.GetTypeCode() + " {\n";

            //    private:
            //        static int typeId;
            //    public:
            //        static int Type(){
            //            return typeId == 0 ? (typeId=YYY::GetTypeId(), typeId) : typeId ;
            //        }
            //        virtual bool IsType(int type){
            //            return typeId == Type() ? true : YYY::IsType(type) ;
            //        }
            texto += "private:\n";
            texto += "\tstatic int typeId\n";
            texto += "public:\n";
            texto += "\tstatic int Type(){\n";
            texto += "\t\treturn typeId == 0 ? (typeId=AI::ReferenciableObject::GetTypeId(), typeId) : typeId; \n";
            texto += "\t}\n";
            texto += "\tvirtual bool IsType(int type) {\n";
            texto += "\t\treturn typeId == Type() ? true : " + BaseObject.GetTypeCode() + "::IsType(type);\n";
            texto += "\t}\n";
            texto += "\n";

            // protected:
            //      XXX(std::string objectName);
            //      ~XXX();
            texto += "protected:\n";
            texto += "\t" + Name + "(std::string objectName);\n";
            texto += "\t~" + Name + "();\n";
            texto += "\n";

            // public:
            //      static void Initialize();
            //      static void Finalize();
            //      static AI::Ref<XXX> New(std::string objectName);
            texto += "public:\n";
            texto += "\tstatic void Initialize();\n";
            texto += "\tstatic void Finalize();\n";
            texto += "\tstatic AI::Ref<" + Name + "> New(std::string objectName);\n";
            texto += "\n";

            //      virtual void Receive(AI::Ref<AI::Message> msg);
            texto += "\tvirtual void Receive(AI::Ref<AI::Message> msg);\n";
            texto += "\n";

            // } ---class---
            texto += "};\n";
            texto += "\n";

            // } ---namespace---
            texto += project.GetNameSpaceCloseCode();
            texto += "\n";

            // #endif
            texto += "#endif // __" + Name + "_H__\n";

            AddText(header, texto);

            header.Close();

        }

        public void GenerateCpp(Proyecto project)
        {
            FileStream cpp = File.Create(project.OutputDir.FullName + System.IO.Path.DirectorySeparatorChar + Name + ".cpp");

            string texto = "";

            // "#include "XXX.h"
            texto += "#include \"" + Name + ".h\"\n";
            texto += "\n";

            // namespace XXX {
            texto += project.GetNameSpaceOpenCode();
            texto += "\n";

            // bool XXX::initialized = false;
            // int XXX:typeId = 0;
            texto += "bool " + Name + "::initialized = false;\n";
            texto += "int " + Name + "::typeId = 0;\n";
            texto += "\n";

            // void
            // XXX::Initialize() {
            //      if (initialized) {
            //          throw AI::Exception::Exception("Already initialized");
            //      }
            // }
            texto += "void\n";
            texto += Name + "::Initialize() {\n";
            texto += "\tif (initialized) {\n";
            texto += "\t\tthrow AI::Exception::Exception(\"Already initialized\")\n";
            texto += "\t}\n";
            texto += "}\n";
            texto += "\n";

            // void
            // XXX:Finalize() {
            //      if (!initialized) {
            //          throw AI::Exception::Exception("Not initialized");
            //      }
            // }
            texto += "void\n";
            texto += Name + "::Finalize() {\n";
            texto += "\tif (!initialized) {\n";
            texto += "\t\tthrow AI::Exception::Exception(\"Not initialized\")\n";
            texto += "\t}\n";
            texto += "}\n";
            texto += "\n";

            // AI::Ref<XXX>
            // XXX::New(std::string objectName) {
            //      XXX *pt = new XXX(objectName);
            //      return pt->GetRef();
            // }
            texto += "AI::Ref<" + Name + ">\n";
            texto += Name + "::New(std::string objectName) {\n";
            texto += "\t" +Name + " *pt = new " + Name + "(objectName);\n";
            texto += "\treturn pt->GetRef();\n";
            texto += "}\n";
            texto += "\n";

            // XXX::XXX(std::string objectName) : YYY(objectName) {
            // }
            texto += Name + "::" + Name + "(std::string objectName) : " + BaseObject.GetTypeCode() + "(objectName) {\n";
            texto += "}\n";
            texto += "\n";

            // XXX::~XXX() {
            // }
            texto += Name + "::~" + Name + "() {\n";
            texto += "}\n";
            texto += "\n";

            // void
            // XXX::Receive(AI::Ref<AI::Message> msg) {
            //      // TODO: Process here your messages.
            //
            //      // If the message is not valid for us, send it to base class
            //      YYY::Receive(msg);
            // }
            texto += "void\n";
            texto += Name + "::Receive(AI::Ref<AI::Message> msg) {\n";
            texto += "\t// TODO: Process here your messages.\n";
            texto += "\n";
            texto += "\t// If the message is not valid for us, send it to base class\n";
            texto += "\t" + BaseObject.GetTypeCode() + "::Receive(msg);\n";
            texto += "}\n";
            texto += "\n";

            // } ---namespace---
            texto += GetNameSpaceCloseCode();
            texto += "\n";

            AddText(cpp, texto);
            cpp.Close();
        }

        public override string GetTypeCode()
        {
            return Name;
        }

        public override string GetIncludeCode()
        {
            return "#include \"" + Name + ".h\"\n";
        }

        public override string GetInitializeCode()
        {
            return Name + "::Initialize();\n";
        }

        public override string GetFinalizeCode()
        {
            return Name + "::Finalize();\n";
        }
    }
}
