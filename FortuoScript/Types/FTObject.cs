using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuoScript.Types
{
    public enum FTType 
    {
        NameDef,
        String,
        Int,
        Bool,
        StartCommandBlock,
        Word,
        EndCommandBlock,
        WordSet
    }

    public class FTObject
    {
        public FTType Type { get; private set; }
        public object Value { get; private set; }

        public FTObject(FTType type, object value)
        {
            Type = type;
            Value = value;
        }

        public string AsWordSet()
        {
            List<FTObject> objs = (List<FTObject>)Value;
            StringBuilder wordset = new StringBuilder();
            foreach (FTObject o in objs)
            {
                if (o.Type == FTType.String)
                    wordset.Append("\"" + o.Value + "\"");
                else if(o.Type == FTType.NameDef)
                    wordset.Append("/" + o.Value);
                else
                    wordset.Append(o.Value);

                if (o != objs.Last())
                    wordset.Append(" ");
            }
            return wordset.ToString();
        }
    }
}