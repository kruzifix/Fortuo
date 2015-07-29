using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FortuoScript.Types
{
    class FTStackUnderFlowException : Exception
    {
        public FTStackUnderFlowException(string msg)
            : base("<-- FTStackUnderFlowException -->\r\n" + msg)
        {  }
    }

    class FTWrongTypeException : Exception
    {
        public FTWrongTypeException(string msg)
            : base("<-- FTWrongTypeException -->\r\n" + msg)
        { }
    }

    class FTUnknownSymbolException : Exception
    {
        public FTUnknownSymbolException(string msg)
            : base("<-- FTUnknownSymbolException -->\r\n" + msg)
        { }
    }
    
    class FTUnexpectedSymbolException : Exception
    {
        public FTUnexpectedSymbolException(string msg)
            : base("<-- FTUnexpectedSymbolException -->\r\n" + msg)
        { }
    }

    class FTWoopsException : Exception
    {
        public FTWoopsException(string msg)
            : base("<-- FTWoopsException -->\r\n" + msg)
        { }
    }
}
