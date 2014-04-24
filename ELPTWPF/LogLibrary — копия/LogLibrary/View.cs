using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogLibrary
{
    public class CView: ICloneable
    {
        /// <summary>
        /// Ссылка на журнал события
        /// </summary>
        public CLog Log { get; private set; }
        /// <summary>
        /// Имя разбиения
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Список трасс разбиения
        /// </summary>
        public List<CTrace> Traces { get; set; }
        /// <summary>
        /// Количество событий разбиения
        /// </summary>
        public int Count { get { int acc = 0; foreach (CTrace trace in Traces) acc += trace.Count; return acc; } }
        /// <summary>
        /// Количество трасс разбиения
        /// </summary>
        public int TraceCount { get { return Traces.Count; } }
        /// <summary>
        /// Самая первая дата в разбиении
        /// </summary>
        DateTimeOffset _Start;
        /// <summary>
        /// Самая последняя дата в разбиении
        /// </summary>
        DateTimeOffset _End;
        /// <summary>
        /// Самая первая дата в разбиении
        /// </summary>
        public DateTimeOffset Start
        {
            get
            {
                try
                {
                    _Start = Traces[0].StartDate;
                    foreach (CTrace trace in Traces)
                        if (trace.StartDate < _Start)
                            _Start = trace.StartDate;
                    return _Start;
                }
                catch (IndexOutOfRangeException) { throw new ArgumentNullException("В данном View нет ни одного события с датой."); }
            }
            private set
            {
                _Start = value;
            }
        }
        /// <summary>
        /// Самая последняя дата в разбиении
        /// </summary>
        public DateTimeOffset End
        {
            get
            {
                try
                {
                    _End = Traces[0].EndDate;
                    foreach (CTrace trace in Traces)
                        if (trace.EndDate > _End)
                            _End = trace.EndDate;
                    return _End;
                }
                catch (IndexOutOfRangeException) { throw new ArgumentNullException("В данном View нет ни одного события с датой."); }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CTrace this[int index] { get { return Traces[index]; } set { Traces[index] = value; } }
        public CView(CLog log) { Traces = new List<CTrace>(); Log = log; }
        public CView(IEnumerable<CTrace> collection, CLog log)
        {
            Traces = new List<CTrace>();
            foreach (CTrace trace in collection)
                Traces.Add((CTrace)trace.Clone());
            Log = log;
        }
        public static CView operator &(CView view1, CView view2)
        {
            if (!view1.Log.Equals(view2.Log))
                throw new ArgumentException("Данные View представляют различные журналы событий");
            CView result = new CView(view1.Log);
            result.Traces = view1.Traces.Intersect(view2.Traces).ToList<CTrace>();
            return result;

        }
        public static CView operator |(CView view1, CView view2)
        {
            if (!view1.Log.Equals(view2.Log))
                throw new ArgumentException("Данные View представляют различные журналы событий");
            List<CTrace> list = new List<CTrace>(view1.Traces);
            list.AddRange(view2.Traces);
            foreach (CTrace trace in view1.Traces.Intersect(view2.Traces))
                list.Remove(trace);
            return new CView(list, view1.Log);
        }
        public static CView operator /(CView view1, CView view2)
        { 
            view1.RemoveAll(x => view2.Traces.Contains(x)); return view1; 
        }
        public void Add(CTrace trace) { Traces.Add(trace); }
        public void AddRange(IEnumerable<CTrace> collection) { Traces.AddRange(collection); }
        public void Remove(CTrace trace) { Traces.Remove(trace); }
        public void RemoveAll(Predicate<CTrace> match) { Traces.RemoveAll(match); }
        public object Clone()
        {
            return new CView(this.Traces, this.Log);
        }
    }
}