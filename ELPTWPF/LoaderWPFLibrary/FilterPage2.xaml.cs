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

namespace WPFSample
{
    /// <summary>
    /// Логика взаимодействия для FilterPage.xaml
    /// </summary>
    public partial class FilterPage2 : UserControl
    {
        List<Type> typeparam = new List<Type>();
        public List<Type> Types
        {
            set
            { typeparam = value; typeparam = value; }
            get
            { return typeparam; }
        }
        Dictionary<string, Type> allparam;
        public Dictionary<string,Type> AllParam
        {
            set
            { allparam = value; StackParams.Children.Clear(); AddElements(value); }
            get
            { return allparam; }
        }

        public void AddElements(Dictionary<string,Type> param)
        {
            foreach (string elem in param.Keys)
            {
                StackPanel sp = new StackPanel();
                Label lb = new Label();
                TextBox tb = new TextBox();
                lb.Content = elem;
                lb.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                lb.Width = 200;
                tb.Width = 200;
                sp.Orientation = Orientation.Horizontal;
                sp.Children.Add(lb);
                sp.Children.Add(tb);
                StackParams.Children.Add(sp);
                typeparam.Add(param[elem]);
            }

        }
        public FilterPage2()
        {
            InitializeComponent();
        }

        public FilterPage2(Dictionary<string,Type> elements)
        {
            InitializeComponent();
            AddElements(elements);
        }
        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            var a = new Dictionary<string, Type>();
            a.Add("one", typeof(String));
            AddElements(a);
        }
    }
}
