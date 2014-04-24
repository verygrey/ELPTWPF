using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogLibrary
{
    public class CEvent: IComparable
    {
        /// <summary>
        /// Ссылка на журнал событий
        /// </summary>
        public CLog _Log { get; private set; }
        /// <summary>
        /// Имя события
        /// </summary>
        string _Name;
        /// <summary>
        /// Имя события
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set 
            {
                try
                {
                    _Log.RemoveEventName(_Name);
                }
                catch (NullReferenceException) { }
                _Log.AddEventName(value); _Name = value; 
            } 
        }
        /// <summary>
        /// ID события
        /// </summary>
        public int EventID { get; private set; }
        /// <summary>
        /// Дата события
        /// </summary>
        public DateTimeOffset? Date { get; set; }
        /// <summary>
        /// Коллекция всех параметров события
        /// </summary>
        public Dictionary<string, object> Parameters
        {
            get
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                foreach (string key in _Text_Parameters.Keys)
                    dict.Add(key, _Text_Parameters[key]);
                foreach (string key in _Int_Parameters.Keys)
                    dict.Add(key, _Int_Parameters[key]);
                foreach (string key in _Double_Parameters.Keys)
                    dict.Add(key, _Double_Parameters[key]);
                foreach (string key in _Bool_Parameters.Keys)
                    dict.Add(key, _Bool_Parameters[key]);
                return dict;
            }
        }
        /// <summary>
        /// Коллекция текстовых параметров
        /// </summary>
        public Dictionary<string, string> Text_Parameters { get { return _Text_Parameters; } }
        /// <summary>
        /// Коллекция текстовых параметров
        /// </summary>
        Dictionary<string, string> _Text_Parameters;
        /// <summary>
        /// Коллекция целочисленных параметров
        /// </summary>
        public Dictionary<string, int> Int_Parameters { get { return _Int_Parameters; } }
        /// <summary>
        /// Коллекция целочисленных параметров
        /// </summary>
        Dictionary<string, int> _Int_Parameters;
        /// <summary>
        /// Коллекция действительных параметров
        /// </summary>
        public Dictionary<string, double> Double_Parameters { get { return _Double_Parameters; } }
        /// <summary>
        /// Коллекция действительных параметров
        /// </summary>
        Dictionary<string, double> _Double_Parameters;
        /// <summary>
        /// Коллекция логических параметров
        /// </summary>
        public Dictionary<string, bool> Bool_Parameters { get { return _Bool_Parameters; } }
        /// <summary>
        /// Коллекция логических параметров
        /// </summary>
        Dictionary<string, bool> _Bool_Parameters;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="log">Журнал событий, в котором будет это событие</param>
        public CEvent(CLog log)
        {
            _Log = log;
            log.Add(this);
            EventID = _Log.LastID++;
            _Name = "";
            _Text_Parameters = new Dictionary<string, string>();
            _Int_Parameters = new Dictionary<string, int>();
            _Double_Parameters = new Dictionary<string, double>();
            _Bool_Parameters = new Dictionary<string, bool>();
        }
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
        /// Сравнивает с другим событием
        /// </summary>
        /// <param name="obj">Событие, которое сравнивается с данным</param>
        public int CompareTo(object obj)
        {
            return Date.Value.CompareTo(((CEvent)obj).Date.Value);
        }
    }
}
