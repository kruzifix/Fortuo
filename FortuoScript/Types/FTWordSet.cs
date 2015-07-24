using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuoScript.Types
{
    class FTWordSet : List<FTObject>
    {
        public FTWordSet()
            : base()
        { }

        public override string ToString()
        {
            StringBuilder set = new StringBuilder();
            this.ForEach(w => {
                switch(w.Type)
                {
                    case FTType.String:
                        set.Append("\"" + w.Value + "\"");
                        break;
                    case FTType.NameDef:
                        set.Append("/" + w.Value);
                        break;
                    case FTType.StartSet:
                        set.Append("{");
                        break;
                    case FTType.EndSet:
                        set.Append("{");
                        break;
                    case FTType.WordSet:
                        set.Append("{ " + w.Value + " }");
                        break;
                    default:
                        set.Append(w.Value);
                        break;
                }
                if (w != this.Last())
                    set.Append(" ");
                });
            return set.ToString();
        }
    }
}