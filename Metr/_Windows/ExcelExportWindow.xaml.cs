using Metr.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Metr._Windows
{
    /// <summary>
    /// Логика взаимодействия для ExcelExportWindow.xaml
    /// </summary>
    public partial class ExcelExportWindow : Window
    {
        List<Column> Columns = new List<Column>();
        EClass localSettings = new EClass();
        public ExcelExportWindow()
        {
            InitializeComponent();

            EClass.UpdPresets();

            TableTypeCmB.ItemsSource = EClass.Presets.Select(p=>p.Name).ToList();
            TableTypeCmB.SelectedIndex = 0;

            UpdateSettings(EClass.Presets[0]);
        }
        void UpdateSettings(EClass a)
        {
            SearchUseChB.IsChecked = a.Settings[0]==1 ? true:false ;
            OriginTableCmB.SelectedIndex = a.Settings[1];
            ObjSortChB.IsChecked = a.Settings[2]==1? true:false ;

            ColumnsDG.ItemsSource = null;
            Columns.Clear();
            for (int i = 0; i < a.CHeader.Count; i++)
             Columns.Add(new Column() { Header = a.CHeader[i], Field = a.Field[i] });
            ColumnsDG.ItemsSource = Columns;            
        }

        private void TableTypeCmB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TableTypeCmB.SelectedIndex > 3)
                OriginTableCmB.IsEnabled = true;
            else
                OriginTableCmB.IsEnabled = false;

            UpdateSettings(EClass.Presets[TableTypeCmB.SelectedIndex]);
        }

        private void expBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                localSettings = EClass.Presets[TableTypeCmB.SelectedIndex];
                localSettings.CHeader = Columns.Select(p => p.Header).ToList();
                localSettings.Field = Columns.Select(p => p.Field).ToList();
                localSettings.Settings = new List<int> { SearchUseChB.IsChecked.Value ? 1 : 0, OriginTableCmB.SelectedIndex, ObjSortChB.IsChecked.Value ? 1 : 0 };
                Thread thread = new Thread(() => EClass.ExportExcel(localSettings)) { IsBackground = true };
                thread.IsBackground = true;
                thread.Start();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
