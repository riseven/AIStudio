using System;
using System.Collections.Generic;
using System.Text;

namespace AIS
{
    class GenericAIObject : AISObject
    {
        private string aiObjectName;
        private string fullAIObjectName;

        public string AIObjectName
        {
            get
            {
                return aiObjectName;
            }
        }
        public string FullAIObjectName
        {
            get
            {
                return fullAIObjectName;
            }
        }

        public GenericAIObject(string aiObjectName, string prefix)
        {
            this.aiObjectName = aiObjectName;
            fullAIObjectName = prefix + aiObjectName;
            Name = aiObjectName;
        }

        public override void GenerateOutputFiles(Proyecto project)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string GetInitializeCode()
        {
            return fullAIObjectName + "::Initialize()\n";
        }

        public override string GetFinalizeCode()
        {
            return fullAIObjectName + "::Finalize()\n";
        }

        public override string GetIncludeCode()
        {
            return "#include \"AICommon.h\"\n";
        }

        public override string GetTypeCode()
        {
            return fullAIObjectName;
        }
    }
}
