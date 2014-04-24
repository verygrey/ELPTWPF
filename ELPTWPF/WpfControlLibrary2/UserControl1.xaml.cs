using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfControlLibrary2
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class DateViewer : UserControl
    {

        private DateTime ThisDateTime;
        private TimeSpan ThisTimeSpan;

        public delegate void ChangeDateEventHandler(DateTimeOffset Date);
        public event ChangeDateEventHandler ChangeDateEvent;


        public DateViewer()
        {
            InitializeComponent();
            this.Hours.LostFocus += Hours_LostFocus;
            this.Minutes.LostFocus += Minutes_LostFocus;
            this.Seconds.LostFocus += Seconds_LostFocus;
            this.MilleSeconds.LostFocus += MilleSeconds_LostFocus;
            this.DatePicker1.SelectedDateChanged += DatePicker1_SelectedDateChanged;
            SetDateTime(DateTimeOffset.Now);
        }


        public DateViewer(DateTimeOffset ThisDateTime)
            : this()
        {
            SetDateTime(ThisDateTime);
        }

        private void SetDateTime(DateTimeOffset ThisDateTime)
        {
            this.ThisDateTime = ThisDateTime.LocalDateTime;
            this.ThisTimeSpan = ThisDateTime.Offset;
            this.Hours.Text = this.ThisDateTime.Hour.ToString("D2");
            this.Minutes.Text = this.ThisDateTime.Minute.ToString("D2");
            this.Seconds.Text = this.ThisDateTime.Second.ToString("D2");
            this.MilleSeconds.Text = this.ThisDateTime.Millisecond.ToString("D3");
            this.DatePicker1.SelectedDate = this.ThisDateTime.Date;
        }

        // Изменение
        #region

        void DatePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ThisDateTime = GetNewTime().LocalDateTime;
            ThisTimeSpan = GetNewTime().Offset;
            if (ChangeDateEvent != null)
                ChangeDateEvent(ThisDateTime);
        }


        void MilleSeconds_LostFocus(object sender, RoutedEventArgs e)
        {
            int millesecond;
            if ((int.TryParse(Hours.Text, out millesecond)) && (millesecond < 1000))
            {
                ThisDateTime = GetNewTime().LocalDateTime;
                ThisTimeSpan = GetNewTime().Offset;
                if (ChangeDateEvent != null)
                    ChangeDateEvent(ThisDateTime);
            }
            this.MilleSeconds.Text = this.ThisDateTime.Millisecond.ToString("D3");
        }

        void Seconds_LostFocus(object sender, RoutedEventArgs e)
        {
            int second;
            if (((int.TryParse(Hours.Text, out second)) && (second < 60) && (second >= 0)))
            {
                ThisDateTime = GetNewTime().LocalDateTime;
                ThisTimeSpan = GetNewTime().Offset;
                if (ChangeDateEvent != null)
                    ChangeDateEvent(ThisDateTime);
            }
            this.Seconds.Text = this.ThisDateTime.Second.ToString("D2");
        }

        void Minutes_LostFocus(object sender, RoutedEventArgs e)
        {
            int minute;
            if (((int.TryParse(Hours.Text, out minute)) && (minute < 60) && (minute >= 0)))
            {
                ThisDateTime = GetNewTime().LocalDateTime;
                ThisTimeSpan = GetNewTime().Offset;
                if (ChangeDateEvent != null)
                    ChangeDateEvent(ThisDateTime);
            }
            this.Minutes.Text = this.ThisDateTime.Minute.ToString("D2");
        }


        private void Hours_LostFocus(object sender, RoutedEventArgs e)
        {
            int hour;
            if (!((int.TryParse(Hours.Text, out hour)) && (hour < 24) && (hour >= 0)))
            {
                ThisDateTime = GetNewTime().LocalDateTime;
                ThisTimeSpan = GetNewTime().Offset;
                if (ChangeDateEvent != null)
                    ChangeDateEvent(ThisDateTime);
            }
            this.Hours.Text = this.ThisDateTime.Hour.ToString("D2");
        }
        #endregion

        public DateTimeOffset GetNewTime()
        {
            DateTime Date = DatePicker1.SelectedDate.Value;
            return new DateTimeOffset(new DateTime(Date.Year, Date.Month, Date.Day, int.Parse(Hours.Text),
                int.Parse(Minutes.Text), int.Parse(Seconds.Text), int.Parse(MilleSeconds.Text)), ThisTimeSpan);
        }

    }

}
