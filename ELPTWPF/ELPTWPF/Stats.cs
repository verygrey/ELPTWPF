using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Threading;
using System.Xml;
using Microsoft.Win32;
using LogLibrary;
using LogLibraryForOpening;
using LoaderLibrary;
using LoaderWPFLibrary;
using WpfControlLibrary2;
using System.Globalization;

namespace ELPTWPF
{
    public partial class MainWindow : Window
    {
        void VisualizeStatsAboutView(CView View)
        {
            //Длинна лога
            int Count = 0;
            foreach (var i in View.Traces)
                Count += i.Count;
            EventCount.Text = Count.ToString();

            //Количество событий
            SortedSet<string> EventNames = new SortedSet<string>();
            foreach (var i in View.Traces)
                foreach (var j in i.Events)
                    EventNames.Add(j.Name);
            EventNameCount.Text = EventNames.Count.ToString();

            // Количество трасс
            TraceCount.Text = View.Traces.Count.ToString();

            // Количество вариантов
            HashSet<string> Variants = new HashSet<string>();
            foreach (var Trace in View.Traces)
            {
                string Variant = "";
                foreach (var Event in Trace.Events)
                {
                    Variant += Event.Name + "=))";
                }
                Variants.Add(Variant);
            }
            VarietyCount.Text = Variants.Count.ToString();

            // Детализация лога
            double Detalization;
            int VarietySum = 0, VarCount = 0;
            foreach (var Trace in NowView.Traces)
            {
                HashSet<string> EventInTraceNames = new HashSet<string>();
                foreach (var Event in Trace.Events)
                    EventInTraceNames.Add(Event.Name);
                VarietySum += EventInTraceNames.Count;
                VarCount++;
            }
            Detalization = (double)VarietySum / (double)VarCount;
            AvarageVariety.Text = Detalization.ToString("F3");

            // Минимальное, среднее и максимальное значение классов событий в trace

            int MinCount = int.MaxValue, MaxCount = 0;
            double AverageCount;
            int Sum = 0, ECount = 0;
            foreach (var Trace in NowView.Traces)
            {
                MinCount = Trace.Events.Count < MinCount ? Trace.Events.Count : MinCount;
                MaxCount = Trace.Events.Count > MaxCount ? Trace.Events.Count : MaxCount;
                Sum += Trace.Events.Count;
                ECount++;
            }
            AverageCount = (double)Sum / (double)ECount;

            MinEventInTraceCount.Text = MinCount.ToString();
            AverageEventInTraceCount.Text = AverageCount.ToString("F2");
            MaxEventInTraceCount.Text = MaxCount.ToString();

            // Минимальное, среднее и максимальное значение количества параметров у события

            int MinParamCount = int.MaxValue, MaxParamCount = 0;
            double AverageParamCount;
            Sum = 0; ECount = 0;
            foreach (var Trace in NowView.Traces)
            {
                for (int i = 0; i < Trace.Events.Count; i++)
                {
                    int ParamCount = Trace[i].Bool_Parameters.Count + Trace[i].Double_Parameters.Count + Trace[i].Int_Parameters.Count +
                        Trace[i].Text_Parameters.Count;
                    MinParamCount = ParamCount < MinParamCount ? ParamCount : MinParamCount;
                    MaxParamCount = ParamCount > MaxParamCount ? ParamCount : MaxParamCount;
                    Sum += ParamCount;
                    ECount++;
                }
            }
            AverageParamCount = (double)Sum / (double)ECount;

            MinParamInEventCount.Text = MinParamCount.ToString();
            AverageParamInEventCount.Text = AverageParamCount.ToString("F2");
            MaxParamInEventCount.Text = MaxParamCount.ToString();

            VisualizeStatsAboutViewGraph(View, EventNames);
        }

        void VisualizeStatsAboutViewGraph(CView View, SortedSet<string> ClassNames)
        {
            // График: по оси ОХ - кейсы, по timestamp первого события, OY - количество классов событий в кейсе -
            Chart1.Series.Clear();
            ObservableCollection<ChartIntTimePoint> ChartData = new ObservableCollection<ChartIntTimePoint>();
            LineSeries NewChart = new LineSeries();
            int Count;
            DateTime ThisTime = new DateTime();
            bool HasDate = false;
            ChartIntTimePoint NewPoint;
            foreach (var i in View.Traces)
            {
                SortedSet<string> EventNames = new SortedSet<string>();
                Count = 0;
                HasDate = false;
                for (int k = 0; k < i.Events.Count; k++)
                {
                    if (HasDate = i[k].Date.HasValue)
                    {
                        ThisTime = i[k].Date.Value.LocalDateTime;
                        break;
                    }
                }
                foreach (var j in i.Events)
                    EventNames.Add(j.Name);
                Count = EventNames.Count;
                if (HasDate)
                {
                    NewPoint = new ChartIntTimePoint();
                    NewPoint.Time = ThisTime;
                    NewPoint.Value = Count;
                    ChartData.Add(NewPoint);
                }
            }
            NewChart.ItemsSource = ChartData;
            NewChart.DependentValuePath = "Value";
            NewChart.IndependentValuePath = "Time";
            NewChart.Title = "Количество классов\nсобытий в трассе";
            Chart1.Series.Add(NewChart);
            // График: по оси ОХ - кейсы, по timestamp первого события, OY - количеств событий в кейсе -
            NewChart = new LineSeries();
            foreach (var i in View.Traces)
            {
                Count = i.Events.Count;
                HasDate = false;
                for (int k = 0; k < i.Events.Count; k++)
                {
                    if (HasDate = i[k].Date.HasValue)
                    {
                        ThisTime = i[k].Date.Value.LocalDateTime;
                        break;
                    }
                }
                if (HasDate)
                {
                    NewPoint = new ChartIntTimePoint();
                    NewPoint.Time = ThisTime;
                    NewPoint.Value = Count;
                    ChartData.Add(NewPoint);
                }
            }
            NewChart.ItemsSource = ChartData;
            NewChart.DependentValuePath = "Value";
            NewChart.IndependentValuePath = "Time";
            NewChart.Title = "Количество\nсобытий в трассе";
            Chart1.Series.Add(NewChart);

            // График: по оси ОХ - классы событий (activity), OY - количество кейсов, в которым встречается это событие -
            NewChart = new LineSeries();
            ObservableCollection<ChartIntStringPoint> ChartDataString = new ObservableCollection<ChartIntStringPoint>();
            ChartIntStringPoint Point = new ChartIntStringPoint();
            foreach (var Name in ClassNames)
            {
                Count = View.Traces.Count<CTrace>(x => x.Events.FindIndex(y => y.Name == Name) != -1);
                Point = new ChartIntStringPoint();
                Point.ClassName = Name;
                Point.Value = Count;
                ChartDataString.Add(Point);
            }
            NewChart.ItemsSource = ChartDataString;
            NewChart.DependentValuePath = "Value";
            NewChart.IndependentValuePath = "ClassName";
            NewChart.Title = "Встречаемость";
            Chart2.Series.Clear();
            Chart2.Series.Add(NewChart);


            // График: по оси ОХ - классы событий (activity), OY - количество кейсов, в которым встречается это событие -
            NewChart = new LineSeries();
            ObservableCollection<ChartDoubleStringPoint> ChartDoubleString = new ObservableCollection<ChartDoubleStringPoint>();
            Point = new ChartIntStringPoint();
            double[] CountOfEv = new double[ClassNames.Count];
            double[] ParamCount = new double[ClassNames.Count];
            foreach (var Trace in View.Traces)
            {
                foreach (var Event in Trace.Events)
                {
                    int Index = ClassNames.ToList<string>().FindIndex(x => x == Event.Name);
                    CountOfEv[Index]++;
                    ParamCount[Index] += Event.Parameters.Count;
                }
            }
            foreach (var Name in ClassNames)
            {
                int Index = ClassNames.ToList<string>().FindIndex(x => x == Name);
                ChartDoubleStringPoint PointD = new ChartDoubleStringPoint();
                PointD.ClassName = Name;
                PointD.Value = ParamCount[Index]/CountOfEv[Index];
                ChartDoubleString.Add(PointD);
            }
            NewChart.ItemsSource = ChartDoubleString;
            NewChart.DependentValuePath = "Value";
            NewChart.IndependentValuePath = "ClassName";
            NewChart.Title = "Количество\nпараметров";
            Chart3.Series.Clear();
            Chart3.Series.Add(NewChart);



        }

        void VisualizeStatsAboutEvent(CEvent Event)
        {
            //Кол-во трасс с выбранным событием
            int Count = 0;
            foreach (var Trace in NowView.Traces)
                if (Trace.Events.FindIndex(x => x.Name == Event.Name) != -1)
                    Count++;
            CaseEventCount.Text = Count.ToString("F2");
            CaseEventPercent.Text = ((double)Count/(double)NowView.TraceCount).ToString();

        }

        void VisualizeStatsAboutTrace(CTrace Trace)
        {
            SortedSet<string> EventNames = new SortedSet<string>();
            foreach (var i in Trace.Events)
                    EventNames.Add(i.Name);
            TraceVarietyCount.Text = (EventNames.Count).ToString();
        }



    }

}