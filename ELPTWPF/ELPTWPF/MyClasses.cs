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
using System.Threading;
using LogLibrary;

namespace ELPTWPF
{
    internal class ChartIntTimePoint
    {
        public int Value { get; set; }
        public DateTime Time { get; set; }
    }

    internal class ChartIntStringPoint
    {
        public int Value { get; set; }
        public string ClassName { get; set; }
    }

    internal class ChartDoubleStringPoint
    {
        public double Value { get; set; }
        public string ClassName { get; set; }
    }  

    internal class Pair
    {
        public int TrIndex { get; set; }
        public int EvIndex { get; set; }
        public Pair()
        {
            TrIndex = -1;
            EvIndex = -1;
        }
        public Pair(int TrIndex, int EvIndex)
        {
            this.TrIndex = TrIndex;
            this.EvIndex = EvIndex;
        }
    }

    enum Params { Text, Bool, Int, Double, NaN, Name };

    internal class NameParam : Pair
    {

        public string Name { get; set; }
        public Params Type { get; set; }
        public NameParam()
            : base()
        {
            Type = Params.NaN;
            Name = "";
        }
        public NameParam(Params Type, int TrIndex, int EvIndex, string Name)
            : base(TrIndex, EvIndex)
        {
            this.Type = Type;
            this.Name = Name;
        }
    }

    internal class BoolComboBox : ComboBox
    {
        public BoolComboBox()
            : base()
        {
            this.IsReadOnly = true;
            this.SelectedIndex = 0;
            ComboBoxItem CItem = new ComboBoxItem();
            CItem.Content = false;
            this.Items.Add(CItem);
            CItem = new ComboBoxItem();
            CItem.Content = true;
            this.Items.Add(CItem);
        }

    }

    internal class ViewButton : Button
    {
        public CView View = new CView(MainWindow.log);
        public DateTime UpdateTime = DateTime.Now;
        public ViewButton()
            : base()
        {
        }
        public ViewButton(CView View)
            : base()
        {
            this.View = View;
        }
    }


    public partial class MainWindow : Window
    {
        internal class MyParamGrid<T> : Grid
        {
            private Params Type;
            public int TrIndex { get; private set; }
            public int EvIndex { get; private set; }
            public MyParamGrid(Dictionary<string, T> Tags, NameParam NP)
                : base()
            {

                this.Type = NP.Type;
                this.TrIndex = NP.TrIndex;
                this.EvIndex = NP.EvIndex;
                this.RowDefinitions.Add(new RowDefinition());
                this.RowDefinitions[this.RowDefinitions.Count - 1].Height = new GridLength(25);
                this.ColumnDefinitions.Add(new ColumnDefinition());
                this.ColumnDefinitions.Add(new ColumnDefinition());
                this.ColumnDefinitions.Add(new ColumnDefinition());
                this.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                this.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                this.ColumnDefinitions[2].Width = new GridLength(25);
                Label First = new Label();
                First.Content = NP.Name;
                First.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                Grid.SetRow(First, 0);
                Grid.SetColumnSpan(First, 2);
                this.Children.Add(First);
                if (Type != Params.Bool)
                    foreach (var i in Tags)
                    {
                        this.RowDefinitions.Add(new RowDefinition());
                        this.RowDefinitions[this.RowDefinitions.Count - 1].Height = new GridLength(25);
                        Label Key = new Label();
                        Key.Content = i.Key;
                        Key.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        Grid.SetRow(Key, this.RowDefinitions.Count - 1);
                        Grid.SetColumn(Key, 0);
                        Key.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                        Key.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                        Key.Margin = new Thickness(0);
                        this.Children.Add(Key);
                        TextBox Value = new TextBox();
                        Value.Background = Brushes.DarkGray;
                        Value.IsReadOnly = true;
                        Value.MouseDoubleClick += OnClick;
                        Value.Text = i.Value.ToString();
                        Value.Tag = new NameParam(Type, TrIndex, EvIndex, i.Key);
                        Value.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                        Value.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                        Value.Margin = new Thickness(3);
                        Grid.SetRow(Value, this.RowDefinitions.Count - 1);
                        Grid.SetColumn(Value, 1);
                        this.Children.Add(Value);
                        Image img = new Image();
                        img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "1.jpg"));
                        Grid.SetColumn(img, 2);
                        Grid.SetRow(img, this.RowDefinitions.Count - 1);
                        this.Children.Add(img);
                        img.Tag = new NameParam(Type, TrIndex, EvIndex, i.Key);
                        img.MouseDown += img_MouseDown;
                    }
                else
                    foreach (var i in Tags)
                    {
                        this.RowDefinitions.Add(new RowDefinition());
                        this.RowDefinitions[this.RowDefinitions.Count - 1].Height = new GridLength(25);
                        Label Key = new Label();
                        Key.Content = i.Key;
                        Key.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        Grid.SetRow(Key, this.RowDefinitions.Count - 1);
                        Grid.SetColumn(Key, 0);
                        Key.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                        Key.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                        Key.Margin = new Thickness(0);
                        this.Children.Add(Key);
                        BoolComboBox Value = new BoolComboBox();

                        Value.IsReadOnly = true;
                        Value.SelectedIndex = (i.Value.ToString() == "true") ? 1 : 0;
                        Value.SelectionChanged += OnChange;
                        //Value.KeyDown += Value_KeyDown;
                        Value.Text = i.Value.ToString();
                        Value.Tag = new NameParam(Params.Bool, TrIndex, EvIndex, i.Key);
                        Value.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                        Value.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                        Value.Margin = new Thickness(3);
                        Grid.SetRow(Value, this.RowDefinitions.Count - 1);
                        Grid.SetColumn(Value, 1);
                        this.Children.Add(Value);
                        Image img = new Image();
                        img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "1.jpg"));
                        Grid.SetColumn(img, 2);
                        Grid.SetRow(img, this.RowDefinitions.Count - 1);
                        this.Children.Add(img);
                        img.Tag = new NameParam(Type, TrIndex, EvIndex, i.Key);
                        img.MouseDown += img_MouseDown;
                    }
            }

            public delegate void ImageClickEventHandler(object sender, MouseButtonEventArgs e);
            public event ImageClickEventHandler ImageClickEvent;

            void img_MouseDown(object sender, MouseButtonEventArgs e)
            {
                if (ImageClickEvent != null)
                    ImageClickEvent(sender, e);
            }

            private void OnChange(object sender, SelectionChangedEventArgs e)
            {
                if (SelChange != null)
                    SelChange(sender, e);
            }

            private void OnClick(object sender, MouseButtonEventArgs e)
            {
                if (ValueDblClick != null)
                    ValueDblClick(sender, e);

            }

            public delegate void Events(object sender, MouseButtonEventArgs e);
            public event Events ValueDblClick;

            public delegate void ChangeEvents(object sender, SelectionChangedEventArgs e);
            public event ChangeEvents SelChange;



        }
    }
}
