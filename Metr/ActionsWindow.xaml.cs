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

namespace Metr
{
    /// <summary>
    /// Логика взаимодействия для ActionsWindow.xaml
    /// </summary>
    public partial class ActionsWindow : Window
    {
        public ActionsWindow(MetrBaseEntities context)
        {
            InitializeComponent();
            mainGrid.ItemsSource = context.Actions.ToList();
        }
    }
}
