using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FortuoScript.Types;

namespace FortuoScript
{
    public class FTStack : Stack<FTObject>
    {
        public FTStack()
            : base()
        { }

        public void Push(FTType type, object value)
        {
            Push(new FTObject(type, value));
        }
    }
}