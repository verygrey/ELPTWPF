using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogLibrary;

namespace Filters
{
    abstract public class AbstractFilter
    {
        public virtual string Name { get { return "noname"; } }
        public virtual string Author { get { return "unknown"; } }
        public virtual Version Ver { get { return new Version(0, 0, 0, 0); } }
        public virtual CView Filter() { throw new TypeLoadException("Фильтрация не содержит объявленного метода фильтрации"); }
        public virtual string Help() { return "I have no help here"; }
    }
}
