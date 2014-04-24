using System;
using System.Collections.Generic;
using System.Windows.Controls.DataVisualization.Charting;
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

namespace ELPTWPF
{
    public partial class MainWindow : Window
    {
        MouseDevice a = null;
        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

            OpenFileName = ((OpenFileDialog)sender).FileName;
            try
            {
                LogOpen.ErrorCountEvent+=LogOpen_ErrorCountEvent;
                LogOpen.openlog(OpenFileName, out log, out trash);
                ClearLists();
                NowView = log.GetView();
                TextBox1.Text = NowView.Traces.Count.ToString();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                ScrollVewer1.ScrollToTop();
                SetViews(((OpenFileDialog)sender).SafeFileName);
                SetTextBoxes();
                SetTreeView(NowView);
                ((Label)StatusBar1.Items[0]).Content = "Лог загружен";
                _Filters.IsEnabled = true;
                VisualizeFromXButton.IsEnabled = true;
                CopyButton.IsEnabled = true;
                DeleteButton.IsEnabled = true;

                this.Title = "ELPTWPF - " + NowView.Name;
            }
            catch (XmlException)
            {
                MessageBox.Show("Неверный XML");
            }
        }

        private void LogOpen_ErrorCountEvent(int ErrorCount)
        {
            MessageBox.Show(String.Format("В журнале событий было найдено {0} ошибок. Программа может выдовать некорректные результаты.", ErrorCount));
        }

        private void SetTreeView(CView View)
        {
            treeView1.Items.Clear();
            VisualizeLog(View, 1, Math.Min(40, View.Traces.Count));
            VisualizeStatsAboutView(View);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void VisualizeLog(CView View, int i, int j)
        {
            for (i--; i < j; i++)
            {
                TreeViewItem Case = new TreeViewItem();
                Case.Header = View.Traces[i].Name;
                Case.ToolTip = (i+1).ToString();
                TreeViewItem events = new TreeViewItem();
                Case.Items.Add(events);
                Case.Expanded += Case_Expanded;
                Case.Collapsed += Case_Collapsed;
                Case.Selected += Case_Selected;
                Case.Tag = i;
                treeView1.Items.Add(Case);
            }
            ((Label)StatusBar1.Items[1]).Visibility = System.Windows.Visibility.Visible;
            ((Label)StatusBar1.Items[1]).Content = "Отображены элементы с " + FirstElement + " по " + j;
        }        

        private void VisualizeParams(int TrIndex, int EvIndex)
        {
            TabControl1.SelectionChanged += TabControl1_SelectionChanged;
            if (EvIndex != -1)
            {
                VisualizeStatsAboutEvent(NowView.Traces[TrIndex].Events[EvIndex]);
                AddName(EvIndex, TrIndex);
                if (NowView.Traces[TrIndex].Events[EvIndex].Date.HasValue)
                    AddDate(NowView.Traces[TrIndex].Events[EvIndex].Date.Value);
                if ((NowView.Traces[TrIndex].Events[EvIndex].Text_Parameters.Count != 0) && (FText.IsChecked))
                {
                    MyParamGrid<string> TextParamGrid = new MyParamGrid<string>(NowView.Traces[TrIndex].Events[EvIndex].Text_Parameters, new NameParam(Params.Text, TrIndex, EvIndex, "Текстовые параметры"));
                    TextParamGrid.ValueDblClick += Value_MouseDoubleClick;
                    TextParamGrid.ImageClickEvent += img_MouseDown;
                    ParamGrid.RowDefinitions.Add(new RowDefinition());
                    ParamGrid.RowDefinitions[ParamGrid.RowDefinitions.Count - 1].Height = new GridLength(TextParamGrid.RowDefinitions.Count * 25);
                    Grid.SetRow(TextParamGrid, ParamGrid.RowDefinitions.Count - 1);
                    Grid.SetColumnSpan(TextParamGrid, 2);
                    ParamGrid.Children.Add(TextParamGrid);
                }
                if ((NowView.Traces[TrIndex].Events[EvIndex].Bool_Parameters.Count != 0) && (FBool.IsChecked))
                {
                    MyParamGrid<bool> BoolParamGrid = new MyParamGrid<bool>(NowView.Traces[TrIndex].Events[EvIndex].Bool_Parameters, new NameParam(Params.Bool, TrIndex, EvIndex, "Логические параметры"));
                    BoolParamGrid.SelChange += BValue_SelectionChanged;
                    BoolParamGrid.ImageClickEvent += img_MouseDown;
                    ParamGrid.RowDefinitions.Add(new RowDefinition());
                    ParamGrid.RowDefinitions[ParamGrid.RowDefinitions.Count - 1].Height = new GridLength(BoolParamGrid.RowDefinitions.Count * 25);
                    Grid.SetRow(BoolParamGrid, ParamGrid.RowDefinitions.Count - 1);
                    Grid.SetColumnSpan(BoolParamGrid, 2);
                    ParamGrid.Children.Add(BoolParamGrid);

                }
                if ((NowView.Traces[TrIndex].Events[EvIndex].Int_Parameters.Count != 0) && (FInt.IsChecked))
                {
                    MyParamGrid<int> IntParamGrid = new MyParamGrid<int>(NowView.Traces[TrIndex].Events[EvIndex].Int_Parameters, new NameParam(Params.Int, TrIndex, EvIndex, "Целочисленные параметры"));
                    IntParamGrid.ValueDblClick += Value_MouseDoubleClick;
                    IntParamGrid.ImageClickEvent += img_MouseDown;
                    ParamGrid.RowDefinitions.Add(new RowDefinition());
                    ParamGrid.RowDefinitions[ParamGrid.RowDefinitions.Count - 1].Height = new GridLength(IntParamGrid.RowDefinitions.Count * 25);
                    Grid.SetRow(IntParamGrid, ParamGrid.RowDefinitions.Count - 1);
                    Grid.SetColumnSpan(IntParamGrid, 2);
                    ParamGrid.Children.Add(IntParamGrid);
                }
                if ((NowView.Traces[TrIndex].Events[EvIndex].Double_Parameters.Count != 0) && (FDouble.IsChecked))
                {
                    MyParamGrid<double> FloatParamGrid = new MyParamGrid<double>(NowView.Traces[TrIndex].Events[EvIndex].Double_Parameters, new NameParam(Params.Int, TrIndex, EvIndex, "Вещественные параметры"));
                    FloatParamGrid.ValueDblClick += Value_MouseDoubleClick;
                    FloatParamGrid.ImageClickEvent += img_MouseDown;
                    ParamGrid.RowDefinitions.Add(new RowDefinition());
                    ParamGrid.RowDefinitions[ParamGrid.RowDefinitions.Count - 1].Height = new GridLength(FloatParamGrid.RowDefinitions.Count * 25);
                    Grid.SetRow(FloatParamGrid, ParamGrid.RowDefinitions.Count - 1);
                    Grid.SetColumnSpan(FloatParamGrid, 2);
                    ParamGrid.Children.Add(FloatParamGrid);
                }
            }
            else
            {
                VisualizeStatsAboutTrace(NowView.Traces[TrIndex]);
                AddName(EvIndex, TrIndex);
                if ((NowView.Traces[TrIndex].Text_Parameters.Count != 0) && (FText.IsChecked))
                {
                    MyParamGrid<string> TextParamGrid = new MyParamGrid<string>(NowView.Traces[TrIndex].Text_Parameters, new NameParam(Params.Text, TrIndex, EvIndex, "Текстовые параметры"));
                    TextParamGrid.ValueDblClick += Value_MouseDoubleClick;
                    TextParamGrid.ImageClickEvent += img_MouseDown;
                    ParamGrid.RowDefinitions.Add(new RowDefinition());
                    ParamGrid.RowDefinitions[ParamGrid.RowDefinitions.Count - 1].Height = new GridLength(TextParamGrid.RowDefinitions.Count * 25);
                    Grid.SetRow(TextParamGrid, ParamGrid.RowDefinitions.Count - 1);
                    Grid.SetColumnSpan(TextParamGrid, 2);
                    ParamGrid.Children.Add(TextParamGrid);
                }
                if ((NowView.Traces[TrIndex].Bool_Parameters.Count != 0) && (FBool.IsChecked))
                {
                    MyParamGrid<bool> BoolParamGrid = new MyParamGrid<bool>(NowView.Traces[TrIndex].Bool_Parameters, new NameParam(Params.Bool, TrIndex, EvIndex, "Логические параметры"));
                    BoolParamGrid.SelChange += BValue_SelectionChanged;
                    BoolParamGrid.ImageClickEvent += img_MouseDown;
                    ParamGrid.RowDefinitions.Add(new RowDefinition());
                    ParamGrid.RowDefinitions[ParamGrid.RowDefinitions.Count - 1].Height = new GridLength(BoolParamGrid.RowDefinitions.Count * 25);
                    Grid.SetRow(BoolParamGrid, ParamGrid.RowDefinitions.Count - 1);
                    Grid.SetColumnSpan(BoolParamGrid, 2);
                    ParamGrid.Children.Add(BoolParamGrid);

                }
                if ((NowView.Traces[TrIndex].Int_Parameters.Count != 0) && (FInt.IsChecked))
                {
                    MyParamGrid<int> IntParamGrid = new MyParamGrid<int>(NowView.Traces[TrIndex].Int_Parameters, new NameParam(Params.Int, TrIndex, EvIndex, "Целочисленные параметры"));
                    IntParamGrid.ValueDblClick += Value_MouseDoubleClick;
                    IntParamGrid.ImageClickEvent += img_MouseDown;
                    ParamGrid.RowDefinitions.Add(new RowDefinition());
                    ParamGrid.RowDefinitions[ParamGrid.RowDefinitions.Count - 1].Height = new GridLength(IntParamGrid.RowDefinitions.Count * 25);
                    Grid.SetRow(IntParamGrid, ParamGrid.RowDefinitions.Count - 1);
                    Grid.SetColumnSpan(IntParamGrid, 2);
                    ParamGrid.Children.Add(IntParamGrid);
                }
                if ((NowView.Traces[TrIndex].Double_Parameters.Count != 0) && (FDouble.IsChecked))
                {
                    MyParamGrid<double> FloatParamGrid = new MyParamGrid<double>(NowView.Traces[TrIndex].Double_Parameters, new NameParam(Params.Int, TrIndex, EvIndex, "Вещественные параметры"));
                    FloatParamGrid.ValueDblClick += Value_MouseDoubleClick;
                    FloatParamGrid.ImageClickEvent += img_MouseDown;
                    ParamGrid.RowDefinitions.Add(new RowDefinition());
                    ParamGrid.RowDefinitions[ParamGrid.RowDefinitions.Count - 1].Height = new GridLength(FloatParamGrid.RowDefinitions.Count * 25);
                    Grid.SetRow(FloatParamGrid, ParamGrid.RowDefinitions.Count - 1);
                    Grid.SetColumnSpan(FloatParamGrid, 2);
                    ParamGrid.Children.Add(FloatParamGrid);
                }
            }
        }

        void TabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (a != null)
            {
                MouseButtonEventArgs ex = new MouseButtonEventArgs(a, -1, MouseButton.Left);
                for (int i = 0; i < EditingValue.Count; i++)
                    Value_MouseDoubleClick(EditingValue[0], ex);
            }
        }

        private void AddName(int EvIndex, int TrIndex)
        {
            ParamGrid.Children.Clear();
            ParamGrid.RowDefinitions.Clear();
            ParamGrid.ColumnDefinitions.Clear();
            ParamGrid.ColumnDefinitions.Add(new ColumnDefinition());
            ParamGrid.ColumnDefinitions.Add(new ColumnDefinition());
            ParamGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            ParamGrid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);

            ParamGrid.RowDefinitions.Add(new RowDefinition());
            ParamGrid.RowDefinitions[ParamGrid.RowDefinitions.Count - 1].Height = new GridLength(25);
            ParamGrid.RowDefinitions.Add(new RowDefinition());
            ParamGrid.RowDefinitions[ParamGrid.RowDefinitions.Count - 1].Height = new GridLength(25);
            Label First = new Label();
            First.Content = "Имя";
            First.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            Grid.SetRow(First, 0);
            Grid.SetColumnSpan(First, 2);
            ParamGrid.Children.Add(First);
            Label Key = new Label();
            Key.Content = "concept:name";
            Key.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            Key.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
            Key.Margin = new Thickness(0);
            Grid.SetRow(Key, 1);
            Grid.SetColumn(Key, 0);
            Grid.SetIsSharedSizeScope(Key, false);
            ParamGrid.Children.Add(Key);
            Key.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
            TextBox Value = new TextBox();
            Value.Background = Brushes.DarkGray;
            Value.IsReadOnly = true;
            Value.MouseDoubleClick += Value_MouseDoubleClick;
            //Value.KeyDown += Value_KeyDown;
            if (EvIndex!=-1)
                Value.Text = NowView.Traces[TrIndex].Events[EvIndex].Name;
            else
                Value.Text = NowView.Traces[TrIndex].Name;
            Value.Tag = new NameParam(Params.Name, TrIndex, EvIndex, "concept:name");
            Value.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            Value.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            Value.Margin = new Thickness(3);
            Grid.SetRow(Value, 1);
            Grid.SetRowSpan(Value, 1);
            Grid.SetColumn(Value, 1);
            Grid.SetColumnSpan(Value, 1);
            ParamGrid.Children.Add(Value);


        }

        void img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show("Вы хотите удалить?", "Вы уверены?", MessageBoxButton.YesNo))
            {
                int TrIndex = ((NameParam)(((Image)sender).Tag)).TrIndex;
                int EvIndex = ((NameParam)(((Image)sender).Tag)).EvIndex;
                string Key = ((NameParam)(((Image)sender).Tag)).Name;
                Params Param = ((NameParam)(((Image)sender).Tag)).Type;
                switch (Param)
                {
                    case Params.Int:
                        log[EvIndex, TrIndex].Int_Parameters.Remove(Key);
                        break;
                    case Params.Double:
                        log[EvIndex, TrIndex].Double_Parameters.Remove(Key);
                        break;
                    case Params.Bool:
                        log[EvIndex, TrIndex].Bool_Parameters.Remove(Key);
                        break;
                    case Params.Text:
                        log[EvIndex, TrIndex].Text_Parameters.Remove(Key);
                        break;
                }
                RefreshParams();
            }

        }

        private void SetTextBoxes()
        {
            TextBox1.Text = NowView.Traces.Count.ToString();
            TextBox2.Text = "1";
        }

        private void setStatusBar()
        {
            Label LoadStatus = new Label();
            LoadStatus.Height = 28;
            LoadStatus.Content = "Лог не загружен";
            StatusBar1.Items.Add(LoadStatus);
            Label LoadNumbers = new Label();
            LoadNumbers.Height = 28;
            LoadNumbers.Visibility = System.Windows.Visibility.Collapsed;
            StatusBar1.Items.Add(LoadNumbers);

            /* Grid Gr1 = new Grid();
             RowDefinition row = new RowDefinition();
             row.Height=new GridLength(35,GridUnitType.Star);
            
             /*Gr1.RowDefinitions.Add(row);
             Gr1.RowDefinitions.Add(row);
             Gr1.RowDefinitions.Add(row);
             Button B = new Button();
             B.Content = "1";*/


        }
        
        private void AddDate(DateTimeOffset Date)
        {

            ParamGrid.RowDefinitions.Add(new RowDefinition());
            ParamGrid.RowDefinitions[ParamGrid.RowDefinitions.Count - 1].Height = new GridLength(73);
            DateViewer CurrentDate = new DateViewer(Date);
            CurrentDate.ChangeDateEvent += ChangeDate;
            Grid.SetRow(CurrentDate, ParamGrid.RowDefinitions.Count - 1);
            Grid.SetColumnSpan(CurrentDate, 2);
            ParamGrid.Children.Add(CurrentDate);
        }

        private void ChangeDate(DateTimeOffset Date)
        {
            log[((Pair)(ParamGrid.Tag)).EvIndex, ((Pair)(ParamGrid.Tag)).TrIndex].Date = Date;
        }

        private void Case_Expanded(object sender, RoutedEventArgs e)
        {
            int TrIndex = treeView1.Items.IndexOf(sender);
            TreeViewItem ExpandedItem = (TreeViewItem)sender;
            CTrace OurTrace = NowView.Traces[TrIndex];
            ExpandedItem.Items.Clear();
            if (OurTrace.Count > 0)
            {
                for (int i = 0; i < OurTrace.Count; i++)
                {
                    TreeViewItem eve = new TreeViewItem();
                    eve.Header = OurTrace[i].Name;
                    eve.Tag = new Pair(TrIndex, NowView.Traces[TrIndex].Events.FindIndex(x => x == OurTrace[i]));
                    eve.Selected += eve_Selected;
                    eve.ToolTip = eve.Header;
                    ExpandedItem.Items.Add(eve);
                }
            }
            else
            {
                TreeViewItem eve = new TreeViewItem();
                eve.Header = @"/*В данноq трассе не содержится событий*/";
                eve.ToolTip ="Нет событий";
                ExpandedItem.Items.Add(eve);
            }

        }

        void Case_Collapsed(object sender, RoutedEventArgs e)
        {
            TreeViewItem ExpandedItem = (TreeViewItem)sender;
            ExpandedItem.Items.Clear();
            TreeViewItem events = new TreeViewItem();
            ExpandedItem.Items.Add(events);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        void eve_Selected(object sender, RoutedEventArgs e)
        {
            TraceVarietyCount.Text = "Выбирите трассу";
            ParamsTab.IsEnabled = true;
            ParamsTabGrid.IsEnabled = true;
            TreeViewItem Sender = (TreeViewItem)sender;
            int TrIndex = ((Pair)Sender.Tag).TrIndex;
            int EvIndex = ((Pair)Sender.Tag).EvIndex;
            ParamGrid.Tag = new Pair(TrIndex, EvIndex);
            VisualizeParams(TrIndex, EvIndex);
        }

        void Case_Selected(object sender, RoutedEventArgs e)
        {
            if (e.Source == sender)
            {
                CaseEventCount.Text = "Выбирите событие";
                CaseEventPercent.Text = "Выбирите событие";
                ParamsTab.IsEnabled = true;
                ParamsTabGrid.IsEnabled = true;
                TreeViewItem Sender = (TreeViewItem)sender;
                int TrIndex = (int)Sender.Tag;
                ParamGrid.Tag = new Pair(TrIndex, -1);
                VisualizeParams(TrIndex, -1);
            }
        }
    }
}
