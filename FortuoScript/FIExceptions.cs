using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortuoScript
{
    class FIStackUnderFlowException : Exception
    {
        public FIStackUnderFlowException(string cmd)
            : base("StackUnderFlow: not enough objects on stack for operation: ["+cmd+"].")
        {  }
    }

    class FIInconsistentTypesException : Exception
    {
        public FIInconsistentTypesException(string cmd)
            :base("InconsistentTypes: unable to process ["+cmd+"].")
        { }
    }

    class FIUnknownSymbolException : Exception
    {
        public FIUnknownSymbolException(string cmd)
            : base("UnknownSymbol: ["+cmd+"]")
        { }
    }

    class FIStackEmptyException : Exception
    {
        public FIStackEmptyException(string cmd)
            : base("Stack Empty: [" + cmd + "]")
        { }
    }
}
