using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogLibrary
{
    public class CLog: IEnumerable<CTrace>
    {
        /// <summary>
        /// Класс словарей событий
        /// </summary>
        public class CEventNames
        {
            Dictionary<string, EventID> _EventNames;
            public int Count { get { return _EventNames.Count; } }
            public List<string> Keys { get { return _EventNames.Keys.ToList<string>(); } }
            public int this[string key] { get { return _EventNames[key].NameID; } }
            public string this[int index] { get { return _EventNames.Keys.ToList<string>().Find(x => _EventNames[x].NameID == index); } }
            class EventID
            {
                static int LastNameID { get; set; }
                public int NameID { get; private set; }
                public int NameCount { get; private set; }
                static EventID() { LastNameID = 0; }
                public EventID() { NameID = LastNameID++; NameCount = 1; }
                public void Increase() { NameCount++; }
                public void Decrease() { NameCount--; }
            }
            public CEventNames() { _EventNames = new Dictionary<string, EventID>(); }
            public void Add(string name)
            {
                try { _EventNames[name].Increase(); }
                catch (KeyNotFoundException) { _EventNames.Add(name, new EventID()); }
            }
            public void Remove(string name)
            {
                try
                {
                    if (_EventNames[name].NameCount > 1) _EventNames[name].Decrease();
                    else _EventNames.Remove(name);
                }
                catch (KeyNotFoundException) { throw new NullReferenceException(); }
            }
            public int NameCount(string key) { return _EventNames[key].NameCount; }
        }

        #region Свойства и поля
        /// <summary>
        /// Версия формата XES
        /// </summary>
        public string XES_version { get; private set; }
        /// <summary>
        /// Страница описания формата XES
        /// </summary>
        public string XES_site { get; private set; }
        /// <summary>
        /// Список событий журнала событий
        /// </summary>
        public List<CEvent> Events { get; set; }
        /// <summary>
        /// Количество событий журнала событий
        /// </summary>
        public int Count { get { return Events.Count; } }
        /// <summary>
        /// Количество трасс в логе
        /// </summary>
        public int TraceCount { get { return _Traces.Traces.Count; } }
        /// <summary>
        /// Разбиение журнала событий на трассы событий
        /// </summary>
        CView _Traces;
        /// <summary>
        /// Словарь событий журнала 
        /// </summary>
        CEventNames _EventNames;
        /// <summary>
        /// Список имен событий
        /// </summary>
        public List<string> EventNames { get { return _EventNames.Keys; } }
        /// <summary>
        /// Количество различных имен событий
        /// </summary>
        public int EventNamesCount { get { return _EventNames.Count; } }
        /// <summary>
        /// Номер последнего ID последнего события
        /// </summary>
        public int LastID { get; set; }
        #endregion

        #region Индексаторы
        /// <summary>
        /// Доступ к трассам по индексу
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CTrace this[int index]
        {
            get { return _Traces.Traces[index]; }
            set { _Traces.Traces[index] = value; }
        }
        /// <summary>
        /// Обращение к событиям журнала событий, по индексу события внутри трассы и номеру трассы
        /// </summary>
        /// <param name="EventIndex">Номер события в трассе</param>
        /// <param name="TraceIndex">Номер трассы. При значении -1 - возвращается событие из всего журнала событий с номером EventIndex</param>
        /// <returns>Событие с номером EventIndex внутри трассы с номером TraceIndex. Если TraceIndex равен -1 возвращается событие номер EventIndex из журнала событий.</returns>
        public CEvent this[int EventIndex, int TraceIndex]
        {
            get { if (TraceIndex == -1)return Events[EventIndex]; else return _Traces.Traces[TraceIndex][EventIndex]; }
        }
        public int this[string key] { get { return _EventNames[key]; } }
        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="XES_version">Версия формата XES</param>
        public CLog(string XES_version = "1.0")
        {
            XES_site = "http://www.xes-standard.org/";
            this.XES_version = XES_version;
            _Traces = new CView(this);
            Events = new List<CEvent>();
            _EventNames = new CEventNames();
        }

        #region Методы редактирования
        /// <summary>
        /// Добавляет новое событие в журнал
        /// </summary>
        /// <param name="evt">Событие</param>
        public void Add(CEvent evt) 
        {
            if (LastID == int.MaxValue) throw new System.OutOfMemoryException();
            Events.Add(evt); 
        }
        /// <summary>
        /// Добавляет новую трассу в разбиение журнала событий
        /// </summary>
        /// <param name="trace">Трасса событий</param>
        public void Add(CTrace trace) { _Traces.Add(trace); }
        /// <summary>
        /// Вставляет указанное событие на указаную позицию в журнале
        /// </summary>
        /// <param name="evt">Вставляемое событие</param>
        /// <param name="index">Индекс, начинающийся с нуля</param>
        public void Insert(CEvent evt, int index) { Events.Insert(index, evt); }
        /// <summary>
        /// Добавляет коллекцию событий
        /// </summary>
        /// <param name="collection">Перечисляемая коллекция событий</param>
        /// <exception cref="System.ArgumentNullException">Параметр collection имеет значение null.</exception>
        public void AddRange(IEnumerable<CEvent> collection) { Events.AddRange(collection); }
        /// <summary>
        /// Удаляет событие
        /// </summary>
        /// <param name="evt">Удаляемое событие</param>
        public void Remove(CEvent evt) { Events.Remove(evt); RemoveEventName(evt.Name); }
        /// <summary>
        /// Удаляет событие.
        /// </summary>
        /// <param name="name">Имя удаляемого события</param>
        public void Remove(string name) { Events.Remove(Events.Find(x => x.Name == name)); RemoveEventName(name); }
        /// <summary>
        /// Удаялет событие.
        /// </summary>
        /// <param name="ID">ID удаляемого события</param>
        public void Remove(int ID) { CEvent evt = Events.Find(x => x.EventID == ID); Events.Remove(evt); RemoveEventName(evt.Name); }
        /// <summary>
        /// Удаляет все события, удаляющие события удовлетворяющие условиям заданного предиката.
        /// </summary>
        /// <param name="match"></param>
        public void RemoveAll(Predicate<CEvent> match) 
        {
            List<CEvent> collection = Events.FindAll(match);
            foreach(CEvent evt in collection)
            {
                Events.Remove(evt); RemoveEventName(evt.Name);
            }
        }
        #endregion

        #region Методы поиска событий
        /// <summary>
        /// Выполняет поиск первого вхождения события.
        /// </summary>
        /// <param name="name">Имя события</param>
        /// <returns>Ищет первое событие с совпадающим именем. Если такового нет, то будет возвращен null.</returns>
        public CEvent Find(string name) { return Events.Find(x => x.Name == name); }
        /// <summary>
        /// Выполняет поиск первого вхождения события.
        /// </summary>
        /// <param name="name">Имя события</param>
        /// <returns>Ищет первое событие с ID. Если такового нет, то будет возвращен null.</returns>
        public CEvent Find(int ID) { return Events.Find(x => x.EventID == ID); }
        /// <summary>
        /// Выподняет поиск всех событий, удовлетворяющих условию заданного предиката.
        /// </summary>
        /// <param name="match">Предикат, определяющий вхождение события в итоговый результат.</param>
        /// <returns>Список всех событий, удовлетворяющих условию заданного предиката.</returns>
        public List<CEvent> FindAll(Predicate<CEvent> match) { return Events.FindAll(match); }
        /// <summary>
        /// Возвращает индекс первого вхождения события.
        /// </summary>
        public int IndexOf(CEvent evt) { return Events.IndexOf(evt); }
        /// <summary>
        /// Возращает индекс первого вхождения трассы.
        /// </summary>
        public int IndexOf(CTrace trace) { return _Traces.Traces.IndexOf(trace); }
        /// <summary>
        /// Возвращает индекс последнего вхождения события.
        /// </summary>
        public int LastIndexOf(CEvent evt) { return Events.LastIndexOf(evt); }
        #endregion

        #region Методы редактирования списка имен событий
        /// <summary>
        /// Добавляет имя события в словарь событий.
        /// </summary>
        /// <param name="name">Имя события</param>
        public void AddEventName(string name) { _EventNames.Add(name); }
        /// <summary>
        /// Удаляет имя события из словаря событий
        /// </summary>
        /// <param name="name">Имя событий</param>
        /// <exception cref="">Имя события не найдено</exception>
        public void RemoveEventName(string name) { _EventNames.Remove(name); }
        /// <summary>
        /// Метод получения имени события по ID
        /// </summary>
        /// <param name="index">ID имени события</param>
        /// <exception cref="ArgumentNullException">Нет имени события с таким ID</exception>
        public string FindEventName(int index) { return _EventNames[index]; }
        /// <summary>
        /// Возвращает количество событий с таким именем
        /// </summary>
        public int NameCount(string name) { return _EventNames.NameCount(name); }
        #endregion

        /// <summary>
        /// Возвращает изначальное разбиение журнала.
        /// </summary>
        public CView GetView()
        {
            return (CView) _Traces.Clone();
        }

        #region Реализация интерфейса IEnumerable
        IEnumerator<CTrace> IEnumerable<CTrace>.GetEnumerator() { return (IEnumerator<CTrace>)GetEnumerator(); }
        public CLogEnumerator GetEnumerator() { return new CLogEnumerator(this._Traces.Traces); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (IEnumerator<CTrace>)GetEnumerator();
        }
        public class CLogEnumerator: IEnumerator<CTrace>
            {
                int position = -1;
                List<CTrace> list;
                public CLogEnumerator(List<CTrace> list) { this.list = list; }
                public CTrace Current
                {
                    get 
                    {
                        try { return list[position]; }
                        catch (IndexOutOfRangeException) 
                        { throw new InvalidOperationException(); }
                    }
                }
                object System.Collections.IEnumerator.Current
                {
                    get { return this.Current; }
                }
                public bool MoveNext()
                {
                    position++;
                    return (position < list.Count);
                }
                public void Reset()
                {
                    position = -1;
                }
                public void Dispose() { }
            }
        #endregion
    }
}
