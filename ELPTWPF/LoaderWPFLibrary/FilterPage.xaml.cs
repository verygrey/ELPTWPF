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
using System.IO;

using LoaderLibrary;
using LogLibrary;

namespace LoaderWPFLibrary
{
    public class ParamBox:TextBox
    {
        string paramname;
        public string ParamName { set { paramname = value; } get { return paramname; } }
        private Type type;
        public Type ThisType { set { type = value; } get { return type; } } 
        public object TrueType
        {
            get
            {
                try
                {
                    if (this.type == typeof(Int32))
                    {
                        return int.Parse(this.Text);
                    }
                    if (this.type == typeof(Int64))
                        return long.Parse(this.Text);
                    if (this.type == typeof(double))
                        return double.Parse(this.Text);
                    if (this.type == typeof(Byte))
                        return byte.Parse(this.Text);
                    if (this.type == typeof(Boolean))
                    {
                        if (this.Text == "Да"||this.Text == "True")
                            return true;
                        else if (this.Text == "Нет"||this.Text == "False")
                            return false;
                        throw new FormatException();
                    }
                    if (this.type == typeof(DateTimeOffset))
                    {
                        int year,month,day,hour,minute,second;
                        string[] days = this.Text.Split(' ')[0].Split('.'), times = this.Text.Split(' ')[1].Split(':');
                        if(days.Length!=3||times.Length!=3)
                            throw new FormatException();
                        year = int.Parse(days[0]);
                        month = int.Parse(days[1]);
                        day = int.Parse(days[2]);
                        hour = int.Parse(times[0]);
                        minute = int.Parse(times[1]);
                        second = int.Parse(times[2]);
                        return new DateTimeOffset(year, month, day, hour, minute, second,TimeSpan.Zero);
                    }
                    return this.Text;
                }
                catch (FormatException)
                {
                    throw new ArgumentException(String.Format("Неверный тип поля {0}, ожидался {1}.", this.paramname, this.ThisType.Name));
                }
            }
        }
    }
    /// <summary>
    /// Логика взаимодействия для FilterPage.xaml
    /// </summary>
    public partial class FilterPage : UserControl
    {
        bool loaded = false;
        bool imagine = false;
        CView currentcview;
        public CView CurrentCView { set { currentcview = value; } get { return currentcview; } }
        string name; int number;
        coord position; 
        FilterStorage FS = new FilterStorage();
        Dictionary<string, Type> allparam;
        List<ParamBox> paramboxes;
        string logdirectory;
        public string LogDirectory { set { logdirectory = value; } get { return logdirectory; } }
        #region constructors
        string viewname;
        public FilterPage()
        {
            InitializeComponent();
            InitPage();
        }
        #endregion
        public void InitPage()
        {
            name = "";
            position = new coord(-1, -1, -1);
            Label lb = new Label();
            lb.Content = "Фильтры не загружены";
            this.StackParams.Children.Add(lb);
        }
        private void ReDrawCombo()
        {
            Names.Items.Clear();
            foreach (string str in FS.Description)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = str;
                Names.Items.Add(cbi);
            }
        }
        /*public void ShowElements(Dictionary<string,Type> param)
        {
            imagine = false;
            StackParams.Children.Clear();
            allparam = param;
            paramboxes = new List<ParamBox>();
            foreach (string elem in allparam.Keys)
            {
                if (allparam[elem] != typeof(CView))
                {
                    StackPanel sp = new StackPanel();
                    Label lb = new Label();
                    ParamBox pb = new ParamBox();
                    lb.Content = elem+"("+allparam[elem].ToString()+")";
                    lb.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                    pb.ParamName = elem;
                    pb.ThisType = allparam[elem];
                    lb.Width = 200;
                    pb.Width = 200;
                    paramboxes.Add(pb);
                    sp.Orientation = Orientation.Horizontal;
                    sp.Children.Add(lb);
                    sp.Children.Add(pb);
                    StackParams.Children.Add(sp);
                }
            }
            imagine = true;
        }*/
        public void ShowElementsGrid(Dictionary<string,Type> param)
        {
            imagine = false;
            StackParams.Children.Clear();
            Label l = new Label();
            l.Content = name;
            StackParams.Children.Add(l);
            allparam = param;
            paramboxes = new List<ParamBox>();
            foreach (string elem in allparam.Keys)
            {
                if (allparam[elem] != typeof(CView))
                {
                    Grid grid = new Grid();
                    RowDefinition rd = new RowDefinition();
                    grid.RowDefinitions.Add(rd);
                    ColumnDefinition cd = new ColumnDefinition();
                    cd.Width = new GridLength(2,GridUnitType.Star);
                    grid.ColumnDefinitions.Add(cd);
                    cd = new ColumnDefinition();
                    cd.Width = new GridLength(3, GridUnitType.Star);
                    grid.ColumnDefinitions.Add(cd);
                    Label lb = new Label();
                    lb.Content = allparam[elem].Name + "["+elem+"]";
                    lb.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                    Grid.SetColumn(lb, 0);
                    ParamBox pb = new ParamBox();
                    pb.ParamName = elem;
                    pb.ThisType = allparam[elem];
                    pb.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                    pb.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    paramboxes.Add(pb);
                    Grid.SetColumn(pb, 1);
                    grid.Children.Add(lb);
                    grid.Children.Add(pb);
                    StackParams.Children.Add(grid);
                }
                else
                {
                    viewname = elem;
                }
            }
            imagine = true;
        }
        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            CView NewView;
            Dictionary<string,object> par = new Dictionary<string,object>();
            try
            {
                if (currentcview != null)
                    par.Add(viewname, currentcview);
                else
                    throw new ArgumentNullException("Отсутствует CView для фильтрации. Пожалуйста, выберите CView.");
                for(int i=0;i<paramboxes.Count;i++)
                {
                    par.Add(paramboxes[i].ParamName,paramboxes[i].TrueType);
                }
                NewView = FS.Run(number, par);
            }
            catch(Exception ex)
            {
                ErrorCatcher(ex); 
                return;
            }            
            currentcview = NewView;
            OnAddingView(NewView);
            ComboBoxItem cbi = new ComboBoxItem();
        }
        public void ShowFiltration(string name, Filtration filtrations)
        {
            this.name = name;
            ShowElementsGrid(filtrations.Types);
        }
        public void ShowFiltration(int number)
        {
 
            ShowElementsGrid(FS[number].Types);
        }
        public void LoadFilters(string path)
        {
            if (!loaded)
            {
                FS.LoadError += ErrorCatcher;
                //FS.LoadError += ErrorLogging;
            }
            loaded = false;
            FS.Load(path);
            ReDrawCombo();
            StackParams.Children.Clear();
            loaded = true;
        }
        private void Names_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(loaded)
                if (FS.Count != 0)
                {
                    number = Names.SelectedIndex;
                    name = FS.LNames[FS.Coords[number].fil];
                    ShowFiltration(number);
                    imagine = true;
                }
        }
        public delegate void AddingViewHandler (CView View);
        public event AddingViewHandler AddingView;
        private void OnAddingView (CView View)
        {
            if (AddingView != null)
                AddingView(View);
        }
        public void ErrorCatcher(Exception ex) { MessageBox.Show(ex.Message); }
        public void ErrorLogging(Exception ex) 
        {
            if (logdirectory != null)
            {
                StreamWriter sw = new StreamWriter(logdirectory + "error.log", true);
                sw.WriteLine(DateTime.Now + ex.Message);
                sw.Close();
            }
        }
        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            if (imagine)
            {
                object ob = FS[name].Help();
                if (ob != null)
                    MessageBox.Show(ob.ToString());
            }
        }
    }
}
