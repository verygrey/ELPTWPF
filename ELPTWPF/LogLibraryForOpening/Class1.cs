using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Xml;
using Microsoft.Win32;
using LogLibrary;
using System.Globalization;


namespace LogLibraryForOpening
{
    public static class LogOpen
    {
        public delegate void ErrorCountEventHandler(int ErrorCount);
        public static event ErrorCountEventHandler ErrorCountEvent;

        private static void open_xml_to_class(XmlDocument doc, out CLog log, out  List<XmlNode> trash)// Открывает XES в класс Log
        {
            int ErrorCount = 0;
            DateTimeOffset? date = null;
            XmlNodeList listoftraces = doc.DocumentElement.ChildNodes;
            XmlNodeList listofevents;
            XmlNodeList listofnames;
            CultureInfo a = new CultureInfo(CultureInfo.CurrentCulture.Name);
            a.NumberFormat.NumberDecimalSeparator = ".";
            log = new CLog();
            trash = new List<XmlNode>();
            foreach (XmlNode trace in listoftraces)
            {
                if (trace.Name == "trace")
                {
                    CTrace tr = new CTrace();
                    listofevents = trace.ChildNodes;
                    foreach (XmlNode node in listofevents)
                    {
                        switch (node.Name)
                        {
                            case "string":
                                if (node.Attributes[0].Value == "concept:name")
                                    tr.Name = node.Attributes[1].Value;
                                else
                                    tr.Text_Parameters.Add(node.Attributes[0].Value, node.Attributes[1].Value);
                                break;
                            case "int":
                                try
                                {
                                    tr.Int_Parameters.Add(node.Attributes[0].Value, int.Parse(node.Attributes[1].Value)); //TODO Это
                                }
                                catch
                                {
                                    ErrorCount++;
                                }
                                break;
                            case "float":
                                try
                                {
                                    tr.Double_Parameters.Add(node.Attributes[0].Value, double.Parse(node.Attributes[1].Value));
                                }
                                catch
                                {
                                    ErrorCount++;
                                }
                                break;
                            case "boolean":
                                try
                                {
                                    tr.Bool_Parameters.Add(node.Attributes[0].Value, bool.Parse(node.Attributes[1].Value));
                                }
                                catch
                                {
                                    ErrorCount++;
                                }
                                break;
                            case "event":
                                CEvent ev = new CEvent(log);
                                listofnames = node.ChildNodes;
                                foreach (XmlNode name in listofnames)
                                    switch (name.Name)
                                    {
                                        case "date":
                                            DateTimeOffset dat = new DateTimeOffset();
                                            DateTimeOffset.TryParse(name.Attributes[1].Value, out dat);
                                            date = dat;
                                            ev.Date = date;
                                            break;
                                        case "string":
                                            if (name.Attributes[0].Value != "concept:name")
                                                ev.Text_Parameters.Add(name.Attributes[0].Value, name.Attributes[1].Value);
                                            else
                                                ev.Name = name.Attributes[1].Value;
                                            break;
                                        case "int":
                                            try
                                            {
                                                ev.Int_Parameters.Add(name.Attributes[0].Value, int.Parse(name.Attributes[1].Value));
                                            }
                                            catch
                                            {
                                                ErrorCount++;
                                            }
                                            break;
                                        case "float":
                                            try
                                            {
                                                ev.Double_Parameters.Add(name.Attributes[0].Value, double.Parse(name.Attributes[1].Value, a));
                                            }
                                            catch
                                            {
                                                ErrorCount++;
                                            }
                                            //TODO Создать логгирующую ошибки системы
                                            break;
                                        case "boolean":
                                            try
                                            {
                                                ev.Bool_Parameters.Add(name.Attributes[0].Value, bool.Parse(name.Attributes[1].Value));
                                            }
                                            catch
                                            {
                                                ErrorCount++;
                                            }
                                            break;
                                    }

                                tr.Add(ev);


                                break;
                        }
                    }
                    log.Add(tr);
                }
                else
                    trash.Add(trace);
            }           
            listofevents = null;
            listofnames = null;
            listoftraces = null;
            if ((ErrorCountEvent != null) && (ErrorCount > 0))
                ErrorCountEvent(ErrorCount);
        }

        public static void openlog(string fileName, out CLog log, out  List<XmlNode> trash)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            open_xml_to_class(doc, out log, out trash);
            doc = null;
        }

        public static void Savelog(string fileName, CLog log, List<XmlNode> trash)
        {
            XmlDocument doc;
            save_xml_from_class(out doc, log, log.GetView(), trash);
            doc.Save(fileName);
            doc = null;
        }

        public static void Savelog(string fileName, CLog log, CView View, List<XmlNode> trash)
        {
            XmlDocument doc;
            save_xml_from_class(out doc, log, View, trash);
            doc.Save(fileName);
            doc = null;
        }

        private static void save_xml_from_class(out XmlDocument doc, CLog log, CView View, List<XmlNode> trash)
        {
            CultureInfo a = new CultureInfo(CultureInfo.CurrentCulture.Name);
            a.NumberFormat.NumberDecimalSeparator = ".";
            doc = new XmlDocument();
            doc.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?> <log> </log>");
            foreach (XmlNode n in trash)
            {
                XmlNode temp = doc.ImportNode(n, true);
                doc.DocumentElement.AppendChild(temp);
            }
            foreach (CTrace trace in View.Traces)
            {
                XmlNode TraceNode = doc.CreateElement("trace");
                XmlNode NameParam = doc.CreateElement("string");
                XmlAttribute Key = doc.CreateAttribute("key"), Value = doc.CreateAttribute("value");
                Key.Value = "concept:name";
                Value.Value = trace.Name;
                NameParam.Attributes.Append(Key);
                NameParam.Attributes.Append(Value);
                TraceNode.AppendChild(NameParam);
                // Выведение параметров Trace
                #region
                foreach (var i in trace.Text_Parameters)
                {
                    XmlNode TextParam = doc.CreateElement("string");
                    Key = doc.CreateAttribute("key"); Value = doc.CreateAttribute("value");
                    Key.Value = i.Key;
                    Value.Value = i.Value.ToString();
                    TextParam.Attributes.Append(Key);
                    TextParam.Attributes.Append(Value);
                    TraceNode.AppendChild(TextParam);
                }
                foreach (var i in trace.Bool_Parameters)
                {
                    XmlNode BoolParam = doc.CreateElement("boolean");
                    Key = doc.CreateAttribute("key"); Value = doc.CreateAttribute("value");
                    Key.Value = i.Key;
                    Value.Value = i.Value.ToString();
                    BoolParam.Attributes.Append(Key);
                    BoolParam.Attributes.Append(Value);
                    TraceNode.AppendChild(BoolParam);
                }
                foreach (var i in trace.Int_Parameters)
                {
                    XmlNode IntParam = doc.CreateElement("int");
                    Key = doc.CreateAttribute("key"); Value = doc.CreateAttribute("value");
                    Key.Value = i.Key;
                    Value.Value = i.Value.ToString();
                    IntParam.Attributes.Append(Key);
                    IntParam.Attributes.Append(Value);
                    TraceNode.AppendChild(IntParam);
                }
                foreach (var i in trace.Double_Parameters)
                {
                    XmlNode FloatParam = doc.CreateElement("float");
                    Key = doc.CreateAttribute("key"); Value = doc.CreateAttribute("value");
                    Key.Value = i.Key;
                    Value.Value = i.Value.ToString(a);
                    FloatParam.Attributes.Append(Key);
                    FloatParam.Attributes.Append(Value);
                    TraceNode.AppendChild(FloatParam);
                }
                #endregion

                foreach (CEvent Event in trace.Events)
                {
                    XmlNode EventNode = doc.CreateElement("event");
                    XmlNode EventNameParam = doc.CreateElement("string");
                    Key = doc.CreateAttribute("key"); Value = doc.CreateAttribute("value");
                    Key.Value = "concept:name";
                    Value.Value = Event.Name;
                    EventNameParam.Attributes.Append(Key);
                    EventNameParam.Attributes.Append(Value);
                    EventNode.AppendChild(EventNameParam);
                    if (Event.Date.HasValue)
                    {
                        DateTimeOffset Date = Event.Date.Value;

                        string date = Date.Year.ToString("D4") + "-" + Date.Month.ToString("D2") + "-" + Date.Day.ToString("D2") + "T" +
                            Date.Hour.ToString("D2") + ":" + Date.Minute.ToString("D2") + ":" + Date.Second.ToString("D2") + "."
                            + Date.Millisecond.ToString("D3") + (Date.Offset.Hours > 0 ? "+" : "-") + Date.Offset.Hours.ToString("D2") +
                            ":" + Date.Offset.Minutes.ToString("D2");
                        XmlNode EventDateParam = doc.CreateElement("date");
                        Key = doc.CreateAttribute("key"); Value = doc.CreateAttribute("value");
                        Key.Value = "time:timestamp";
                        Value.Value = date;
                        EventDateParam.Attributes.Append(Key);
                        EventDateParam.Attributes.Append(Value);
                        EventNode.AppendChild(EventDateParam);
                    }
                    foreach (var i in Event.Text_Parameters)
                    {
                        XmlNode TextParam = doc.CreateElement("string");
                        Key = doc.CreateAttribute("key"); Value = doc.CreateAttribute("value");
                        Key.Value = i.Key;
                        Value.Value = i.Value.ToString();
                        TextParam.Attributes.Append(Key);
                        TextParam.Attributes.Append(Value);
                        EventNode.AppendChild(TextParam);
                    }
                    foreach (var i in Event.Bool_Parameters)
                    {
                        XmlNode BoolParam = doc.CreateElement("boolean");
                        Key = doc.CreateAttribute("key"); Value = doc.CreateAttribute("value");
                        Key.Value = i.Key;
                        Value.Value = i.Value.ToString();
                        BoolParam.Attributes.Append(Key);
                        BoolParam.Attributes.Append(Value);
                        EventNode.AppendChild(BoolParam);
                    }
                    foreach (var i in Event.Int_Parameters)
                    {
                        XmlNode IntParam = doc.CreateElement("int");
                        Key = doc.CreateAttribute("key"); Value = doc.CreateAttribute("value");
                        Key.Value = i.Key;
                        Value.Value = i.Value.ToString();
                        IntParam.Attributes.Append(Key);
                        IntParam.Attributes.Append(Value);
                        EventNode.AppendChild(IntParam);
                    }
                    foreach (var i in Event.Double_Parameters)
                    {
                        XmlNode FloatParam = doc.CreateElement("float");
                        Key = doc.CreateAttribute("key"); Value = doc.CreateAttribute("value");
                        Key.Value = i.Key;
                        Value.Value = i.Value.ToString(a);
                        FloatParam.Attributes.Append(Key);
                        FloatParam.Attributes.Append(Value);
                        EventNode.AppendChild(FloatParam);
                    }
                    TraceNode.AppendChild(EventNode);
                }
                doc.DocumentElement.AppendChild(TraceNode);
            }
        }
    }
}
