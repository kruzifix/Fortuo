using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortuoScript
{
    public class FTNameDef
    {
        public string Name { get; private set; }

        public FTNameDef(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return string.Format("NameDef: {0}", Name);
        }
    }
}