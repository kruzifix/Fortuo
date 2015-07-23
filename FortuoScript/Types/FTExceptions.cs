using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortuoScript.Types
{
    class FTStackUnderFlowException : Exception
    {
        public FTStackUnderFlowException(string cmd)
            : base("StackUnderFlow: not enough objects on stack for operation: ["+cmd+"].")
        {  }
    }

    class FTWrongTypeException : Exception
    {
        public FTWrongTypeException(string cmd)
            :base("WrongType: unable to process ["+cmd+"].")
        { }
    }

    class FTUnknownSymbolException : Exception
    {
        public FTUnknownSymbolException(string cmd)
            : base("UnknownSymbol: ["+cmd+"]")
        { }
    }
    
    class FTUnexpectedSymbolException : Exception
    {
        public FTUnexpectedSymbolException(string cmd)
            : base("UnexpectedSymbol: [" + cmd + "]")
        { }
    }

    class FTWoopsException : Exception
    {
        public FTWoopsException(string cmd)
            : base("Woops!  [" + cmd + "]")
        { }
    }
}
