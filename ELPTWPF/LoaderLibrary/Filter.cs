using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using LogLibrary;
using System.IO;
using Filters;

namespace LoaderLibrary
{ 
    public class TypeIncorrectException:TypeLoadException
    {
        public TypeIncorrectException() : base() { }
        public TypeIncorrectException(string message) : base(message) { }
        public TypeIncorrectException(string message, Exception inner) : base(message, inner) { }
    }
    public class Filter
    {
        object filter;
        public Assembly assembly;
        MethodInfo help;
        List<Filtration> filtrations;
        public string author;
        public string path;
        public string name;
        public string libname;
        public Version version;
        public override string ToString()
        {
            return String.Format("{0} ver.{1} by {2} in library {3}", this.name, this.version, this.author, this.libname);
        }
        public int Count { get { return filtrations.Count; } }
        public Filtration this[int i] { get { return filtrations[i]; } }
        public Filter()
        {
        }
        /// <summary>
        /// Создает на основе типа фильтр
        /// </summary>
        /// <param name="lib"></param>
        /// <param name="type"></param>
        public Filter(string lib, Type type)
        {
            libname = lib;
            if (type.BaseType == typeof(AbstractFilter))
            {
                Exception fullex = new Exception("");
                ConstructorInfo ci = type.GetConstructor(System.Type.EmptyTypes);
                filter = ci.Invoke(null);
                PropertyInfo everyprop;
                everyprop = type.GetProperty("Name");
                name = (string)everyprop.GetValue(filter, null);
                everyprop = type.GetProperty("Author");
                author = (string)everyprop.GetValue(filter, null);
                everyprop = type.GetProperty("Ver");
                version = (Version)everyprop.GetValue(filter, null);
                help = type.GetMethod("Help");
                MethodInfo[] methods = type.GetMethods();
                filtrations = new List<Filtration>();
                foreach (MethodInfo mi in methods)
                    if (mi.Name == "Filter")
                    {
                        try
                        {
                            filtrations.Add(new Filtration(mi));
                        }
                        catch (TypeLoadException)
                        {
                            //Не добавляем фильтрацию.
                        }
                    }
                if (filtrations == null) throw new TypeIncorrectException("Класс " + name + " не содержит ни одной фильтрации");
            }
            else
                throw new TypeLoadException("Класс " + type.Name + " не наследует AbstractFilter");
        }
        /// <summary>
        /// Запускает фильтр
        /// </summary>
        /// <param name="number">Номер фильтрации</param>
        /// <param name="lob">Словарь параметров</param>
        /// <returns></returns>
        public CView Run(int number, Dictionary<string,object> dictparam)
        {
            return filtrations[number].Run(filter, dictparam);
        }
        /// <summary>
        /// Вызывает помощь
        /// </summary>
        /// <returns></returns>
        public object Help()
        {
            return help.Invoke(filter, null);
        }
    }
}
