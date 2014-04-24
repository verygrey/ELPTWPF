using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LogLibrary;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

using Filters;

namespace LoaderLibrary
{
    public struct coord
    {
        public int abs, fil, rel;
        /// <summary>
        /// Координаты фильтрации
        /// </summary>
        /// <param name="abs">Номер по порядку</param>
        /// <param name="fil">Номер фильтра</param>
        /// <param name="rel">Номер в фильтре</param>
        public coord(int abs, int fil, int rel)
        {
            this.abs = abs;
            this.fil = fil;
            this.rel = rel;
        }
    }
    public class FilterStorage
    {
        bool loaded;
        public bool Loaded { get { return loaded; } }
        
        List<Filter> filters;
        public long Count { get { return filters.Count; } }

        List<coord> coords;
        public List<coord> Coords { get { return coords; } }

        List<string> lnames;
        Dictionary<string,int> names;
        List<string> description;

        public List<string> LNames { get { return lnames; } }
        public Dictionary<string,int> Names { get { return names; } }
        public List<string> Description { get { return description; } }
        
        string directory;
        public string Directory { get { return directory; } }

        public FilterStorage()
        {
            loaded = false;
            filters = new List<Filter>();
            coords = new List<coord>();
            lnames = new List<string>();
            names = new Dictionary<string,int>();
            description = new List<string>();
            directory = "";
        }
        public FilterStorage(string path)
        {
            string[] pathes = System.IO.Directory.GetFiles(path, "*.dll");
            Filter f;
            int i = 0, j = 0;
            foreach (string _path in pathes)
            {
                Assembly assembly = Assembly.LoadFrom(_path);
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    try
                    {
                        f = new Filter(System.IO.Path.GetFileNameWithoutExtension(_path), type);
                        filters.Add(f);
                        for (int k = 0; k < f.Count; k++)
                        {
                            description.Add("#" + j.ToString() + ": " + f.name + "." + k);
                            coords.Add(new coord(i, j, k));
                        }
                        j++;
                    }
                    catch (TypeLoadException ex)
                    {
                        Exception e = new Exception("В типе " + type.Name + " возникли ошибки:\n" + ex.Message);
                        Warning(e);
                    }
                }
            }
            this.directory = path;
            loaded = true;
        }
        public void Load(string path)
        {
            loaded = false;
            string[] pathes = System.IO.Directory.GetFiles(path, "*.dll");
            Filter f;
            int i = 0, j = 0;
            foreach (string _path in pathes)
            {
                Assembly assembly = Assembly.LoadFrom(_path);
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    try
                    {
                        f = new Filter(System.IO.Path.GetFileNameWithoutExtension(_path), type);
                        filters.Add(f);
                        lnames.Add(type.Name);
                        names.Add(type.Name,j);
                        for (int k = 0; k < f.Count; k++)
                        {
                            description.Add("#" + i.ToString() + ": " + type.Name + "." + k);
                            coords.Add(new coord(i++, j, k));
                        }
                        j++;
                    }
                    catch (TypeIncorrectException)
                    {

                    }
                    catch (TypeLoadException ex)
                    {
                        //Exception e = new Exception("В типе " + type.Name + " возникли ошибки:\n" + ex.Message);
                        //Warning(e);
                    }
                }
            }
            this.directory = path;
            loaded = true;
        }

        public Filtration this[int i] { get { return this.filters[this.coords[i].fil][this.coords[i].rel]; } }
        public Filter this[string s] { get { return filters[names[s]]; } }
        
        public override string ToString()
        {
            if (!loaded)
                return "Фильтры не загружены";
            string answer = directory + "\n";
            for (int i = 0; i < Count; i++)
                answer += "\n" + filters[i].ToString();
            return answer;
        }
        public CView Run(int i, Dictionary<string, object> param)
        { 
            return filters[coords[i].fil].Run(coords[i].rel,param);
        }
        
        public delegate void Error(Exception ex);
        public event Error LoadError;
        private void Warning(Exception ex)
        {
            if (LoadError != null)
                LoadError(ex);
        }
    }
}
