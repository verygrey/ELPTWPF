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




    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static CLog log = new CLog();
        private int FirstElement = 1;
        private Window1 AddWin;
        public CView NowView = new CView(log);
        private string OpenFileName, SaveFileName;
        private List<XmlNode> trash = new List<XmlNode>();
        private bool HasEditingValue;
        FilterPage NewFilterPage;
        private List<TextBox> EditingValue = new List<TextBox>();

        public MainWindow()
        {
            InitializeComponent();
            TextBox1.Text = "0";
            setStatusBar();
            ScrollVewer1.ScrollChanged += ScrollVewer1_ScrollChanged;
        }

        private void VerifyNewParam(string key, string value, Params Type)
        {
            int EvIndex = ((Pair)ParamGrid.Tag).EvIndex, TrIndex = ((Pair)ParamGrid.Tag).TrIndex;

            switch (Type)
            {
                case Params.Text:
                    if (!(((EvIndex != -1) && (NowView.Traces[TrIndex].Events[EvIndex].Text_Parameters.Keys.Contains(key))) || (((EvIndex == -1) && (NowView.Traces[TrIndex].Text_Parameters.Keys.Contains(key))))))
                    {
                        AddWin.Close();
                        AddWin = null;
                        if (EvIndex != -1)
                            NowView.Traces[TrIndex].Events[EvIndex].Text_Parameters.Add(key, value);
                        else
                            NowView.Traces[TrIndex].Text_Parameters.Add(key, value);
                        RefreshParams();
                    }
                    else
                    {
                        MessageBox.Show("Параметр с таким именем уже существует");
                        AddWin.Activate();
                    }
                    break;
                case Params.Bool:
                    if (!(((EvIndex != -1) && (NowView.Traces[TrIndex].Events[EvIndex].Bool_Parameters.Keys.Contains(key))) || (((EvIndex == -1) && (NowView.Traces[TrIndex].Bool_Parameters.Keys.Contains(key))))))
                    {
                        AddWin.Close();
                        AddWin = null;
                        if (EvIndex != -1)
                            NowView.Traces[TrIndex].Events[EvIndex].Bool_Parameters.Add(key, bool.Parse(value));
                        else
                            NowView.Traces[TrIndex].Bool_Parameters.Add(key, bool.Parse(value));
                        RefreshParams();
                    }
                    else
                    {
                        MessageBox.Show("Параметр с таким именем уже существует");
                        AddWin.Activate();
                    }
                    break;
                case Params.Int:
                    if (!(((EvIndex != -1) && (NowView.Traces[TrIndex].Events[EvIndex].Int_Parameters.Keys.Contains(key))) || (((EvIndex == -1) && (NowView.Traces[TrIndex].Int_Parameters.Keys.Contains(key))))))
                    {
                        AddWin.Close();
                        AddWin = null;
                        if (EvIndex != -1)
                            NowView.Traces[TrIndex].Events[EvIndex].Int_Parameters.Add(key, int.Parse(value));
                        else
                            NowView.Traces[TrIndex].Int_Parameters.Add(key, int.Parse(value));
                        RefreshParams();
                    }
                    else
                    {
                        MessageBox.Show("Параметр с таким именем уже существует");
                        AddWin.Activate();
                    }
                    break;
                case Params.Double:
                    if (!(((EvIndex != -1) && (NowView.Traces[TrIndex].Events[EvIndex].Double_Parameters.Keys.Contains(key))) || (((EvIndex == -1) && (NowView.Traces[TrIndex].Double_Parameters.Keys.Contains(key))))))
                    {
                        AddWin.Close();
                        AddWin = null;
                        if (EvIndex != -1)
                            NowView.Traces[TrIndex].Events[EvIndex].Double_Parameters.Add(key, double.Parse(value));
                        else
                            NowView.Traces[TrIndex].Double_Parameters.Add(key, double.Parse(value));
                        RefreshParams();
                    }
                    else
                    {
                        MessageBox.Show("Параметр с таким именем уже существует");
                        AddWin.Activate();
                    }
                    break;


            }
        }

        private void SetViews(string ShortFileName)
        {
            ViewListGrid.RowDefinitions.Clear();
            ViewListGrid.ColumnDefinitions.Clear();
            ViewListGrid.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            ViewListGrid.ColumnDefinitions.Add(new ColumnDefinition());
            ViewListGrid.RowDefinitions.Add(new RowDefinition());
            ViewListGrid.RowDefinitions[0].Height = new GridLength(40);
            CView View = NowView;
            ViewButton GeneralView = new ViewButton(View);
            GeneralView.Height = 40;
            GeneralView.Click += ChangeView;
            GeneralView.Background = Brushes.DarkGray;
            GeneralView.Content = "\nОбновлено в " + GeneralView.UpdateTime.ToShortTimeString();
            GeneralView.Margin = new Thickness(0);
            GeneralView.View.Name = ShortFileName;
            GeneralView.Content = GeneralView.View.Name + "\nОбновлено в " + GeneralView.UpdateTime.ToShortTimeString();
            ViewListGrid.Margin = new Thickness(0);
            ViewListGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            Grid.SetRow(GeneralView, 0);
            ViewListGrid.Children.Add(GeneralView);
            //NowView = GeneralView.View;
        }

        void NewFilterPage_AddingView(CView View)
        {
            string s = null;
            ViewListGrid.RowDefinitions.Add(new RowDefinition());
            ViewListGrid.RowDefinitions[0].Height = new GridLength(40);
            ViewButton ButtonView = new ViewButton(View);
            string Header = s != null ? s : View.Name + "_Copy";//\nОбновлено в " + ButtonView.UpdateTime.ToShortTimeString();

            if ((ViewListGrid.Children.Cast<ViewButton>()).FirstOrDefault(x => x.View.Name == Header) != null)
            {
                int i = 1;
                for (; ((ViewListGrid.Children.Cast<ViewButton>()).FirstOrDefault(x => x.View.Name == Header + "_" + i) != null); i++) ;
                ButtonView.View.Name = Header + "_" + i;
            }
            else
                ButtonView.View.Name = Header;
            ButtonView.Height = 40;
            ButtonView.Content = ButtonView.View.Name + "\nОбновлено в " + ButtonView.UpdateTime.ToShortTimeString();
            ButtonView.Margin = new Thickness(0);
            ButtonView.Click += ChangeView;
            ViewListGrid.Margin = new Thickness(0);
            ViewListGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            Grid.SetRow(ButtonView, ViewListGrid.RowDefinitions.Count - 1);
            ViewListGrid.Children.Add(ButtonView);
        }

        private void ChangeView(object sender, RoutedEventArgs e)
        {
            ((ViewListGrid.Children.Cast<ViewButton>()).First(x => x.View == NowView)).Background = Color1.Clone();
            NowView = ((ViewButton)sender).View;
            ((ViewButton)sender).Background = Brushes.DarkGray;
            SetTreeView(NowView);
            if (NewFilterPage!=null)
            NewFilterPage.CurrentCView = NowView;
            this.Title = "ELPTWPF - " + NowView.Name;
        }


        void ScrollVewer1_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            if ((treeView1.Items.Count < (NowView.Traces.Count - FirstElement + 1)) && (ScrollVewer1.ScrollableHeight - ScrollVewer1.ContentVerticalOffset == 0))
            {
                VisualizeLog(NowView, treeView1.Items.Count + FirstElement - 1, Math.Min(treeView1.Items.Count + FirstElement - 1 + 20, NowView.Traces.Count));
            }
        }


        private void _Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void _Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = @"S:\";
            openFileDialog1.Filter = "XES | *.xes";
            openFileDialog1.FileOk += openFileDialog1_FileOk;
            openFileDialog1.ShowDialog();

        }

        void Value_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            a = e.MouseDevice;
            TextBox SenderTextbox = ((TextBox)sender);
            if (SenderTextbox.IsReadOnly)
            {
                SenderTextbox.IsReadOnly = false;
                SenderTextbox.Background = Brushes.White;
                HasEditingValue = true;
                EditingValue.Add(SenderTextbox);
            }
            else
            {
                Params Type = ((NameParam)SenderTextbox.Tag).Type;
                string Name = ((NameParam)SenderTextbox.Tag).Name;
                int EvIndex = ((NameParam)SenderTextbox.Tag).EvIndex, TrIndex = ((NameParam)SenderTextbox.Tag).TrIndex;
                switch (Type)
                {
                    case Params.Name:
                        if ((SenderTextbox.Text != ""))
                        {
                            if (EvIndex != -1)
                            {
                                NowView.Traces[TrIndex].Events[EvIndex].Name = SenderTextbox.Text;
                                if (((TreeViewItem)treeView1.Items[TrIndex]).IsExpanded)
                                    ((TreeViewItem)(((TreeViewItem)treeView1.Items[TrIndex]).Items[EvIndex])).Header = SenderTextbox.Text;
                            }
                            else
                            {
                                NowView.Traces[TrIndex].Name = SenderTextbox.Text;
                                ((TreeViewItem)treeView1.Items[TrIndex]).Header = SenderTextbox.Text;
                            }
                            SenderTextbox.IsReadOnly = true;
                            SenderTextbox.Background = Brushes.DarkGray;
                            EditingValue.Remove(SenderTextbox);
                            if (EditingValue.Count == 0)
                                HasEditingValue = true;
                        }
                        else
                            MessageBox.Show("Именем event не может быть пустая строка");
                        break;
                    case Params.Text:
                        log[EvIndex, TrIndex].Text_Parameters[Name] = SenderTextbox.Text;
                        SenderTextbox.IsReadOnly = true;
                        SenderTextbox.Background = Brushes.DarkGray;
                        EditingValue.Remove(SenderTextbox);
                        if (EditingValue.Count == 0)
                            HasEditingValue = true;
                        break;
                    case Params.Int:
                        try
                        {
                            if (EvIndex != -1)
                                NowView.Traces[TrIndex].Events[EvIndex].Int_Parameters[Name] = int.Parse(SenderTextbox.Text);
                            else
                                NowView.Traces[TrIndex].Int_Parameters[Name] = int.Parse(SenderTextbox.Text);
                            SenderTextbox.IsReadOnly = true;
                            SenderTextbox.Background = Brushes.DarkGray;
                            EditingValue.Remove(SenderTextbox);
                            if (EditingValue.Count == 0)
                                HasEditingValue = true;
                        }
                        catch (FormatException ex)
                        {
                            MessageBoxResult result = MessageBoxResult.Yes;
                            if (e.Timestamp != -1)
                                result = MessageBox.Show("Вы ввели новое неверное значение. Хотите ли вы вернуть первоначальное значение?", "Ошибка!", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                SenderTextbox.Text = EvIndex != -1 ? NowView.Traces[TrIndex].Events[EvIndex].Int_Parameters[Name].ToString() : NowView.Traces[TrIndex].Int_Parameters[Name].ToString();
                                SenderTextbox.IsReadOnly = true;
                                SenderTextbox.Background = Brushes.DarkGray;
                                EditingValue.Remove(SenderTextbox);
                                if (EditingValue.Count == 0)
                                    HasEditingValue = true;
                            }
                        }
                        break;
                    case Params.Double:
                        try
                        {
                            if (EvIndex != -1)
                                NowView.Traces[TrIndex].Events[EvIndex].Double_Parameters[Name] = double.Parse(SenderTextbox.Text);
                            else
                                NowView.Traces[TrIndex].Double_Parameters[Name] = double.Parse(SenderTextbox.Text);
                            SenderTextbox.IsReadOnly = true;
                            SenderTextbox.Background = Brushes.DarkGray;
                            EditingValue.Remove(SenderTextbox);
                            if (EditingValue.Count == 0)
                                HasEditingValue = true;
                        }
                        catch (FormatException)
                        {
                            MessageBoxResult result = MessageBoxResult.Yes;
                            if (e.Timestamp != -1)
                                result = MessageBox.Show("Вы ввели новое неверное значение. Хотите ли вы вернуть первоначальное значение?", "Ошибка!", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                SenderTextbox.Text = EvIndex != -1 ? NowView.Traces[TrIndex].Events[EvIndex].Double_Parameters[Name].ToString() : NowView.Traces[TrIndex].Double_Parameters[Name].ToString();
                                SenderTextbox.IsReadOnly = true;
                                SenderTextbox.Background = Brushes.DarkGray;
                                EditingValue.Remove(SenderTextbox);
                                if (EditingValue.Count == 0)
                                    HasEditingValue = true;
                            }
                        }
                        break;

                }
            }
        }

        void BValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BoolComboBox SenderCombobox = ((BoolComboBox)sender);
            Params Type = ((NameParam)SenderCombobox.Tag).Type;
            string Name = ((NameParam)SenderCombobox.Tag).Name;
            int EvIndex = ((NameParam)SenderCombobox.Tag).EvIndex, TrIndex = ((NameParam)SenderCombobox.Tag).TrIndex;
            if (EvIndex != -1)
                NowView.Traces[TrIndex].Events[EvIndex].Bool_Parameters[Name] = SenderCombobox.SelectedIndex == 1 ? true : false;
            else
                NowView.Traces[TrIndex].Bool_Parameters[Name] = SenderCombobox.SelectedIndex == 1 ? true : false;
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)//Загрузка Trace
        {
            try
            {
                int i = int.Parse(TextBox2.Text);
                if (i > NowView.Traces.Count)
                {
                    MessageBox.Show("Номер за пределами диапозона");
                }
                else
                {
                    treeView1.Items.Clear();
                    ScrollVewer1.ScrollToVerticalOffset(0);
                    FirstElement = i;
                    VisualizeLog(NowView, i, Math.Min(i + 40, NowView.Traces.Count));
                }
            }
            catch 
            {
                MessageBox.Show("Указано неверное значение. Необходимо ввести целое число.");
            }


            GC.Collect();
            GC.WaitForPendingFinalizers();

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (AddWin == null)
            {
                AddWin = new Window1();
                AddWin.Show();
                AddWin.Closing += AddWin_Closing;
                AddWin.GoodParam += VerifyNewParam;
            }
            else AddWin.Activate();

        }

        void AddWin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AddWin = null;
        }

        private void Changed(object sender, RoutedEventArgs e)
        {
            if ((ParamGrid != null) && (ParamsTabGrid.IsEnabled))
            {
                RefreshParams();
            }
        }

        private void RefreshParams()
        {
            Pair Coords = (Pair)(ParamGrid.Tag);
            VisualizeParams(Coords.TrIndex, Coords.EvIndex);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog Dialog = new System.Windows.Forms.FolderBrowserDialog();
            Dialog.SelectedPath = @"S:\\";
            if ((Dialog.ShowDialog()) == System.Windows.Forms.DialogResult.OK)
            {
                AddFilterPage(Dialog.SelectedPath);
                FilterTabName.IsEnabled = true;
                NewFilterPage.CurrentCView = NowView;
            }
            //TODO сделать по-человечески:)
        }

        private void AddFilterPage(string path)
        {
            NewFilterPage = new FilterPage();
            NewFilterPage.AddingView += NewFilterPage_AddingView;
            NewFilterPage.Margin = new Thickness(0);
            Grid.SetColumn(NewFilterPage, 0);
            Grid.SetRow(NewFilterPage, 0);
            NewFilterPage.LoadFilters(path);
            FilterGrid.Children.Add(NewFilterPage);
        }

        private void CopyNowView(object sender, RoutedEventArgs e)
        {
            NewFilterPage_AddingView((CView)(NowView.Clone()));
        }

        private void DeleteSelectedView(object sender, RoutedEventArgs e)
        {
            int index = ((ViewListGrid.Children.Cast<ViewButton>()).ToList<ViewButton>()).FindIndex(x => x.View == NowView);
            if (index != 0)
            {
                for (int j = index + 1; j < ViewListGrid.RowDefinitions.Count; j++)
                    Grid.SetRow(ViewListGrid.Children[j], j - 1);
                ViewListGrid.RowDefinitions.RemoveAt(ViewListGrid.RowDefinitions.Count - 1);
                UIElement[] Collection = new UIElement[ViewListGrid.Children.Count - 1];
                for (int i = 0; i < ViewListGrid.Children.Count - 1; i++)
                    if (i < index)
                        Collection[i] = ViewListGrid.Children[i];
                    else
                        Collection[i] = ViewListGrid.Children[i + 1];
                ViewListGrid.Children.Clear();
                foreach (var i in Collection)
                    if (i != null)
                        ViewListGrid.Children.Add(i);
                NowView = ((ViewButton)(ViewListGrid.Children[0])).View;
                ((ViewButton)(ViewListGrid.Children[0])).Background = Brushes.DarkGray;
                SetTreeView(NowView);
                NewFilterPage.CurrentCView = NowView;
                this.Title = "ELPTWPF - " + NowView.Name;
            }
            else
                MessageBox.Show("Нельзя удалить основное View");

        }

        private void _Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.Filter = "XES |*.xes";
            SaveFileDialog1.FileOk += saveFileDialog1_FileOk;
            SaveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveFileName = ((SaveFileDialog)sender).FileName;
            LogOpen.Savelog(SaveFileName, log, NowView, trash);
            TextBox1.Text = NowView.Traces.Count.ToString();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void ClearMemory_MouseDoubleClick_1(object sender, RoutedEventArgs e)
        {
            // Очистка всего и вся
            ClearLists();
            log = new CLog();
            NowView = new CView(log);
            trash = new List<XmlNode>();

            //Начальная доступность
            FilterTabName.IsEnabled = false;
            ParamsTab.IsEnabled = false;
            _Filters.IsEnabled = false;
            CopyButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            VisualizeFromXButton.IsEnabled = false;
            _Save.IsEnabled = false;

            //Установка строки состояния

            Label LoadStatus = new Label();
            LoadStatus.Height = 28;
            LoadStatus.Content = "Лог не загружен";
            StatusBar1.Items.Add(LoadStatus);
            Label LoadNumbers = new Label();
            LoadNumbers.Height = 28;
            LoadNumbers.Visibility = System.Windows.Visibility.Collapsed;
            StatusBar1.Items.Add(LoadNumbers);
            

            // Очистка памяти
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void ClearLists()
        {
            ParamGrid.Children.Clear();
            treeView1.Items.Clear();
            ViewListGrid.Children.Clear();

            // Начальные значения для текст боксов
            TextBox1.Text = "";
            TextBox2.Text = "";
            EventCount.Text = "";
            EventNameCount.Text = "";
            TraceCount.Text = "";
            VarietyCount.Text = "";
            CaseEventCount.Text = "Выбирите событие";
            CaseEventPercent.Text = "Выбирите событие";

            // Очистка памяти
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void CaseEventCount_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}