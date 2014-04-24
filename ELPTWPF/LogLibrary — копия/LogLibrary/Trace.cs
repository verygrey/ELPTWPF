using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogLibrary
{
    public class CTrace: ICloneable
    { 
        /// <summary>
        /// Имя трассы
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Список событий трассы
        /// </summary>
        public List<CEvent> Events { get; set; }
        /// <summary>
        /// Текстовые параметры трассы
        /// </summary>
        Dictionary<string, string> _Text_Parameters;
        /// <summary>
        /// Текстовые параметры трассы
        /// </summary>
        public Dictionary<string, string> Text_Parameters { get { return _Text_Parameters; } }
        /// <summary>
        /// Целочисленные параметры трассы
        /// </summary>
        Dictionary<string, int> _Int_Parameters;
        /// <summary>
        /// Целочисленные параметры трассы
        /// </summary>
        public Dictionary<string, int> Int_Parameters { get { return _Int_Parameters; } }
        /// <summary>
        /// Дробные параметры трассы
        /// </summary>
        Dictionary<string, double> _Double_Parameters;
        /// <summary>
        /// Дробные параметры трассы
        /// </summary>
        public Dictionary<string, double> Double_Parameters { get { return _Double_Parameters; } }
        /// <summary>
        /// Логические параметры
        /// </summary>
        Dictionary<string, bool> _Bool_Parameters;
        /// <summary>
        /// Логические параметры
        /// </summary>
        public Dictionary<string, bool> Bool_Parameters { get { return _Bool_Parameters; } }
        /// <summary>
        /// Самая первая дата в трассе
        /// </summary>
        public DateTimeOffset StartDate
        {
            get
            {
                DateTimeOffset start;
                int index = 0;
                while (index < Count && Events[index++].Date == null) ;
                if (index == Count) throw new ArgumentNullException();
                start = Events[--index].Date.Value;
                index++;
                for (; index < Count; index++)
                    if (Events[index] != null && start > Events[index].Date.Value)
                        start = Events[index].Date.Value;
                return start;
            }
        }
        /// <summary>
        /// Самая последняя дата
        /// </summary>
        public DateTimeOffset EndDate
        {
            get
            {
                DateTimeOffset end;
                int index = 0;
                while (index < Count && Events[index++].Date == null) ;
                if (index == Count) throw new ArgumentNullException();
                end = Events[--index].Date.Value;
                index++;
                for (; index < Count; index++)
                    if (Events[index] != null && end < Events[index].Date.Value)
                        end = Events[index].Date.Value;
                return end;
            }
        }
        /// <summary>
        /// Возвращает событие по индексу
        /// </summary>
        /// <param name="index">Индекс, начинающийся с нуля.</param>
        public CEvent this[int index] { get { return Events[index]; } }
        /// <summary>
        /// Данный индексатор позволяет записывать значения параметров, независимо от их типа.
        /// </summary>
        /// <param name="key">Название параметра.</param>
        /// <returns>Значение параметра по ключу типа object.</returns>
        public object this[string key]
        {
            get
            {
                try { return _Int_Parameters[key]; }
                catch (KeyNotFoundException)
                {
                    try { return _Double_Parameters[key]; }
                    catch (KeyNotFoundException)
                    {
                        try { return _Bool_Parameters[key]; }
                        catch (KeyNotFoundException)
                        {
                            return _Text_Parameters[key];
                        }
                    }
                }
            }
            set
            {
                try
                {
                    if (value is Int32 | _Int_Parameters.Keys.Contains(key)) _Int_Parameters[key] = (int)value;
                    else if (value is Double | _Double_Parameters.Keys.Contains(key)) _Double_Parameters[key] = (double)value;
                    else if (value is Boolean | _Bool_Parameters.Keys.Contains(key)) _Bool_Parameters[key] = (bool)value;
                    else if (value is String | _Text_Parameters.Keys.Contains(key)) _Text_Parameters[key] = value.ToString();
                    else throw new ArgumentException();
                }
                catch (InvalidCastException) { throw new ArgumentException(); }
            }
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        public CTrace()
        {
            Events = new List<CEvent>(); _Text_Parameters = new Dictionary<string, string>(); _Bool_Parameters = new Dictionary<string, bool>();
        _Int_Parameters = new Dictionary<string, int>(); _Double_Parameters = new Dictionary<string, double>();
        }
        /// <summary>
        /// Количество событий в трассе
        /// </summary>
        public int Count { get { return Events.Count; } }
        /// <summary>
        /// Добавляет событие в трассу
        /// </summary>
        public void Add(CEvent evt) { Events.Add(evt); }
        /// <summary>
        /// Добавляет перечислимую коллекцию событий в трассу
        /// </summary>
        public void AddRange(IEnumerable<CEvent> collection) 
        {
            foreach (CEvent evt in collection)
                Events.Add(evt);
        }
        /// <summary>
        /// Удаляет событие
        /// </summary>
        public void Remove(CEvent evt) { Events.Remove(evt); }
        /// <summary>
        /// Удаляет все события, удовлетворяющие условиям указанного предиката
        /// </summary>
        public void RemoveAll(Predicate<CEvent> match) { Events.RemoveAll(match); }
        /// <summary>
        /// Добавить параметр
        /// </summary>
        /// <param name="parameterName">Имя параметра</param>
        /// <param name="value">Значение параметра</param>
        public void AddParameter(string parameterName, int value) 
        {
            if (_Double_Parameters.ContainsKey(parameterName) || _Text_Parameters.ContainsKey(parameterName) || _Bool_Parameters.ContainsKey(parameterName))
                throw new ArgumentException();
            _Int_Parameters.Add(parameterName, value); 
        }
        /// <summary>
        /// Добавить параметр
        /// </summary>
        /// <param name="parameterName">Имя параметра</param>
        /// <param name="value">Значение параметра</param>
        public void AddParameter(string parameterName, double value) 
        {
            if (_Int_Parameters.ContainsKey(parameterName) || _Text_Parameters.ContainsKey(parameterName) || _Bool_Parameters.ContainsKey(parameterName))
                throw new ArgumentException();
            _Double_Parameters.Add(parameterName, value); 
        }
        /// <summary>
        /// Добавить параметр
        /// </summary>
        /// <param name="parameterName">Имя параметра</param>
        /// <param name="value">Значение параметра</param>
        public void AddParameter(string parameterName, string value) 
        {
            if (_Double_Parameters.ContainsKey(parameterName) || _Int_Parameters.ContainsKey(parameterName) || _Bool_Parameters.ContainsKey(parameterName))
                throw new ArgumentException();
            _Text_Parameters.Add(parameterName, value); 
        }
        /// <summary>
        /// Добавить параметр
        /// </summary>
        /// <param name="parameterName">Имя параметра</param>
        /// <param name="value">Значение параметра</param>
        public void AddParameter(string parameterName, bool value) 
        {
            if (_Double_Parameters.ContainsKey(parameterName) || _Text_Parameters.ContainsKey(parameterName) || _Int_Parameters.ContainsKey(parameterName))
                throw new ArgumentException();
            _Bool_Parameters.Add(parameterName, value); 
        }
        /// <summary>
        /// Создает клона данной трассы
        /// </summary>
        public object Clone()
        {
            CTrace trace = new CTrace();
            trace.Name = Name;
            foreach (CEvent evt in this.Events)
                trace.Add(evt);
            foreach (string key in _Bool_Parameters.Keys)
                trace[key] = this[key];
            foreach (string key in _Int_Parameters.Keys)
                trace[key] = this[key];
            foreach (string key in _Double_Parameters.Keys)
                trace[key] = this[key];
            foreach (string key in _Text_Parameters.Keys)
                trace[key] = this[key];
            return trace;
        }

        
    }
}
