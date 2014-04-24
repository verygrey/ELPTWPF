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
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace ELPTWPF
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            Key.TextChanged += Key_TextChanged;
            Value.TextChanged += Value_TextChanged;           
        }

        void Value_TextChanged(object sender, TextChangedEventArgs e)
        {
            BoolComboBox BComboBox = new BoolComboBox();
            BComboBox.RenderSize = Value.RenderSize;
            BComboBox.Margin = Value.Margin;
            //BComboBox.
            switch (TComboBox.SelectedIndex)
            {
                case 1:
                    Value.Visibility = System.Windows.Visibility.Visible;
                    if ((Value.Text.Length > 0) && (Value.Text[0] != ' ') && (Value.Text[Value.Text.Length - 1] != ' '))
                    {
                        AddButton.IsEnabled = true;
                    }
                    else
                    {
                        AddButton.IsEnabled = false;
                    }

                    break;
                case 2:
                    bool f;
                    Value.Visibility = System.Windows.Visibility.Hidden;
                    
                    
                    break;
                case 3:
                     int i;
                     Value.Visibility = System.Windows.Visibility.Visible;
                    if (int.TryParse(Value.Text,out i))
                    {
                        AddButton.IsEnabled = true;
                    }
                    else
                    {
                        AddButton.IsEnabled = false;
                    }

                    break;
                case 4:
                    double d;
                    Value.Visibility = System.Windows.Visibility.Visible;
                    if (double.TryParse(Value.Text,out d))
                    {
                        AddButton.IsEnabled = true;
                    }
                    else
                    {
                        AddButton.IsEnabled = false;
                    }
                    break;
            } 
        }

        void Key_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TComboBox.SelectedIndex != 2)
            {
                if ((Key.Text.Length > 0) && (Key.Text[0] != ' ') && (Key.Text[Key.Text.Length - 1] != ' '))
                {
                    Value.IsEnabled = true;
                    Value.Text = "";
                }
                else
                {
                    Value.IsEnabled = false;
                    Value.Text = "";
                    AddButton.IsEnabled = false;
                }
            }
            else
                if ((Key.Text.Length > 0) && (Key.Text[0] != ' ') && (Key.Text[Key.Text.Length - 1] != ' '))
                    AddButton.IsEnabled = true; 
                else
                    AddButton.IsEnabled = false;
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (TComboBox.SelectedIndex != 0)
            {
                Key.IsEnabled = true;
                Key.Text = "";
                if (TComboBox.SelectedIndex == 2)
                {
                    //Value.Visibility = System.Windows.Visibility.Collapsed;
                    BBox.Visibility = System.Windows.Visibility.Visible;
                    
                }
                else
                {
                   // Value.Visibility = System.Windows.Visibility.Visible;
                    BBox.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            else
            {
                Key.IsEnabled = false;
                Key.Text = "";
                Value.IsEnabled = false;
                Value.Text = "";
                AddButton.IsEnabled = false;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        internal delegate void GoodParamHandler( string key, string value, Params Type);
        internal event GoodParamHandler GoodParam;

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (GoodParam != null)
            {
                GoodParam(Key.Text,TComboBox.SelectedIndex!=2 ? Value.Text : ((ComboBoxItem)BBox.SelectedItem).Content.ToString(), ((Params)(TComboBox.SelectedIndex-1)));
            }
        }       
    }
}
