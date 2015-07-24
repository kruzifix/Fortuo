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
        StartSet,
        Word,
        EndSet,
        WordSet
    }

    public class FTObject
    {
        public FTType Type { get; private set; }
        public object Value { get; set; }

        public FTObject(FTType type, object value)
        {
            Type = type;
            Value = value;
        }
        
        public override string ToString()
        {
            return string.Format("[{0}]  ->  {1}", Type, Value);
        }
    }
}