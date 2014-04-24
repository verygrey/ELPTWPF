using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using LogLibrary;

namespace LoaderLibrary
{
    public class Filtration
    {
        MethodInfo work;
        Dictionary<string, int> ordinal;
        Dictionary<string, Type> types;
        public Dictionary<string, Type> Types { get { return types; } }
        public Filtration()
        { }
        public Filtration(MethodInfo targetmethod)
        {
            if(targetmethod.Name!="Filter") throw new TypeLoadException("Фильтрация имеет имя, отличное от Filter");
            if(targetmethod.ReturnType!=typeof(CView)) throw new TypeLoadException("Фильтрация возвращает тип, отличный от CView");
            ParameterInfo[] paramS = targetmethod.GetParameters();
            bool found = false;
            ordinal = new Dictionary<string,int>();
            types = new Dictionary<string, Type>();
            foreach(ParameterInfo pi in paramS)
            {
                ordinal.Add(pi.Name,pi.Position);
                types.Add(pi.Name, pi.ParameterType);
                if(pi.ParameterType==typeof(CView)) found=true;
            }
            if(!found) throw new TypeLoadException("Фильтрация не имеет параметра типа CView");
            work = targetmethod;           
        }
        public CView Run(object filterlib, Dictionary<string,object> inputparams)
        {
            object[] runparams = new object[ordinal.Count];
            foreach (string name in inputparams.Keys)
                runparams[ordinal[name]] = inputparams[name];
            return (CView)work.Invoke(filterlib, runparams);
        }
    }
}
