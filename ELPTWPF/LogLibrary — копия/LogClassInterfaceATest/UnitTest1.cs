using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogLibrary;
using System.IO;

namespace LogClassInterfaceATest
{
    public class Methods
    {
        static public string GenerateString(ref Random rand, int length = 15)
        {
            string str = "";
            int len = rand.Next(1, length);
            for (int i = 0; i < len; i++)
                str += (char)rand.Next('a', 'z' + 1);
            return str;
        }
        static public CEvent GenerateEvent(CLog log, ref Random rand)
        {
            CEvent evt = new CEvent(log);
            evt.Name = GenerateString(ref rand);
            evt.Date = GenerateDateTimeOffset(ref rand);
            int paramsCount = rand.Next(0, 15);
            for (int i = 0; i < paramsCount; i++)
                try { evt[GenerateString(ref rand, 7)] = evt.Name; }
                catch (ArgumentException) { File.AppendAllText(@"C:\Users\Fedor\Документы ВШЭ\Программирование\Курсовая\Log ver 2 Test Results\Error Log.txt", "Попытка присвоить значение другого типа\r\n"); }
            paramsCount = rand.Next(0, 15);
            for (int i = 0; i < paramsCount;i++ )
                try { evt[GenerateString(ref rand, 10)] = rand.Next(); }
                catch (ArgumentException) { File.AppendAllText(@"C:\Users\Fedor\Документы ВШЭ\Программирование\Курсовая\Log ver 2 Test Results\Error Log.txt", "Попытка присвоить значение другого типа\r\n"); }
            paramsCount = rand.Next(0, 15);
            for (int i = 0; i < paramsCount;i++ )
                try { evt[GenerateString(ref rand)] = rand.NextDouble() + rand.Next(); }
                catch (ArgumentException) { File.AppendAllText(@"C:\Users\Fedor\Документы ВШЭ\Программирование\Курсовая\Log ver 2 Test Results\Error Log.txt", "Попытка присвоить значение другого типа\r\n"); }
            return evt;
        }
        static public DateTimeOffset? GenerateDateTimeOffset(ref Random rand)
        {
            DateTimeOffset? date;
            if (rand.Next(0, 2) == 0)
                date = new DateTimeOffset(rand.Next(2000, 2015), rand.Next(1, 13), rand.Next(1, 28), rand.Next(0, 24), rand.Next(0, 60), rand.Next(0, 60), new TimeSpan());
            else
                date = null;
            return date;
        }
        static public CLog GenerateLog(int count = 100)
        {
            CLog log = new CLog();
            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                Methods.GenerateEvent(log, ref rand);
                log[i,-1].Date = Methods.GenerateDateTimeOffset(ref rand);
            }
            return log;
        }
        static public CTrace FilledTrace(ref Random rand, CLog log)
        {
            CTrace trace = new CTrace();
            int count = rand.Next(5, 15);
            for (int i = 0; i < count; i++)
            {
                trace.Add(GenerateEvent(log, ref rand));
            }
            return trace;
        }

    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            const int count = 100;
            Random rand = new Random();
            CLog log = new CLog();
            for (int i = 0; i < count; i++)
            {
                Methods.GenerateEvent(log, ref rand);
                log[i,-1].Date = Methods.GenerateDateTimeOffset(ref rand);
            }
            Assert.AreEqual(count, log.Count);
        }

        [TestMethod]
        public void TestMethod2()
        {
            CLog log = new CLog();
            CEvent evt = new CEvent(log);
            evt.Name = "test1";
            Assert.AreEqual("test1", log[0,-1].Name);
        }

        [TestMethod]
        public void TestMethod3()
        {
            CLog log = new CLog();
            CEvent evt = new CEvent(log);
            log[0,-1].Name = "test1";
            Assert.AreEqual("test1", evt.Name);
        }

        [TestMethod]
        public void TestMethod4()
        {
            CLog log = new CLog();
            CEvent evt;
            for (int i=0;i<100;i++)
                evt = new CEvent(log);
            for (int i=0;i<100;i++)
                Assert.AreEqual(i,log[i,-1].EventID);

        }

        [TestMethod]
        public void TestMethod5()
                {
                    const int count = 10;
                    Random rand = new Random();
                    CLog log = new CLog();
                    for (int i = 0; i < count; i++)
                    {
                        Methods.GenerateEvent(log, ref rand);
                        log[i,-1].Date = Methods.GenerateDateTimeOffset(ref rand);
                    }
                    log.Add(new CTrace());
                    CEvent evt2=log.Events[0];
                    foreach (CEvent evt in log.Events)
                        log[0].Add(evt);
                    CView view = log.GetView();
                    view[0].Remove(evt2);
                    Assert.AreEqual(log[0].Count - 1, view[0].Count);
                    
                }

        [TestMethod]
        public void TestMethod6()
        {
            Random rand = new Random(1);
            CLog log = Methods.GenerateLog(1000);
            int count = log.Count;
            CTrace trace;
            while (count > 0)
            {
                trace = Methods.FilledTrace(ref rand,log);
                log.Add(trace);
                count -= log[log.TraceCount - 1].Count;
            }
            File.WriteAllText("C:/test.txt", log.TraceCount.ToString());
        }

        /// <summary>
        /// Проверка работоспособности списков параметров класса CEvent
        /// </summary>
        [TestMethod]
        public void CEventParametersTest()
        {
            CLog log = Methods.GenerateLog();
            string text= "";
            foreach (CEvent evt in log.Events)
            {
                text += "Int Parameters:\r\n";
                foreach (string key in evt.Int_Parameters.Keys)
                    text += evt[key] + "\r\n";
                text += "Double Parameters:\r\n";
                foreach (string key in evt.Double_Parameters.Keys)
                    text += evt[key] + "\r\n";
                text += "Text Parameters:\r\n";
                foreach (string key in evt.Text_Parameters.Keys)
                    text += evt[key] + "\r\n";
                text += "\r\n";
            }
            Assert.AreEqual(true, log[0, -1].Text_Parameters.ContainsValue(log[0, -1].Name));
            File.WriteAllText(String.Format(@"C:\Users\Fedor\Документы ВШЭ\Программирование\Курсовая\Log ver 2 Test Results\CEventParametersTest,{3}_{4}_{0}_{1}_{2}.txt", 
                DateTime.Now.Day.ToString(),DateTime.Now.Month.ToString(),DateTime.Now.Year.ToString(),DateTime.Now.Hour.ToString(),DateTime.Now.Minute.ToString()),text);
        }

        /// <summary>
        /// Проверка работоспособности списки имен событий класса CLog
        /// </summary>
        [TestMethod]
        public void CLogEventNamesTest()
        {
            CLog log = new CLog();
            string name = "";
            for (int i=1;i<15;i++)
            {
                name += "a";
                for (int j = 0; j < i; j++)
                {
                    CEvent evt = new CEvent(log);
                    evt.Name = name;
                }
            }
            Assert.AreEqual(14, log.EventNamesCount);
            name = "";
            foreach (string key in log.EventNames)
                name += key + "\r\n";
            File.WriteAllText(String.Format(@"C:\Users\Fedor\Документы ВШЭ\Программирование\Курсовая\Log ver 2 Test Results\CLogEventNamesTest{3}_{4}_{0}_{1}_{2}.txt", 
                DateTime.Now.Day.ToString(),DateTime.Now.Month.ToString(),DateTime.Now.Year.ToString(),DateTime.Now.Hour.ToString(),DateTime.Now.Minute.ToString()), name);
        }
        
        [TestMethod]
        public void DateTest()
        {
            CLog log = Methods.GenerateLog(1);

            Assert.AreNotEqual(false, log[0, -1].Date.Value);
        }
    }
}
