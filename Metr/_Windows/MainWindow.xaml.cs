using Metr._Windows;
using Metr.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Metr
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MetrBaseEntities context = MetrBaseEntities.GetContext();
        List<string> dSearch = new List<string>();
        List<string> objects = new List<string>();
        DateTime? searchStart = null;
        DateTime? searchEnd = null;
        bool searchDef;
        bool searchDel;
        bool pprDate;
        bool Exp;

        public CurrentUser User { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            logIn();
            Thread thread = new Thread(UpdateTabs) { IsBackground = true };
            thread.Start();
        }
        //Метод запроса авторизации
        void logIn()
        {
            AuthWindow authWin = new AuthWindow();
            authWin.ShowDialog();

            switch (authWin.DialogResult)
            {
                case true:
                    if (authWin.authUser.Id != 1)
                    {
                        User = new CurrentUser() { Id = authWin.authUser.Id, FullName = authWin.authUser.FullName, RoleID = authWin.authUser.RoleID };
                        MessageBox.Show("Вы вошли как " + User.FullName + "\nДобро пожаловать!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        User = new CurrentUser() { Id = 1, FullName = "Гость", RoleID = 1 };
                        MessageBox.Show("В режиме просмотра вам не доступны действия связанные с редактированием", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;

                case false:
                    Close();
                    break;
            }
        }
        //Метод обновления вкладок
        void UpdateTabs()
        {
            try
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    pBar.Visibility = Visibility.Visible;
                }));

                DeviceData.dataUpdate();

                Dispatcher.Invoke(new Action(() =>
                {
                    pBar.Visibility = Visibility.Visible;

                    BottomUpdate();

                    deviceGrid.ItemsSource = null;
                    deviceGrid.ItemsSource = DeviceData.deviceListMain;

                    pprGrid.ItemsSource = null;
                    pprGrid.ItemsSource = DeviceData.deviceListPPR;

                    excGrid.ItemsSource = null;
                    excGrid.ItemsSource = DeviceData.deviceListExc;

                    pBar.Visibility = Visibility.Collapsed;
                    searchTBObj.ItemsSource = (context.Object.OrderBy(d => d.Name).Select(d => d.Name).ToList());
                }));
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    pBar.Value = 0;
                    pBar.Visibility = Visibility.Collapsed;
                }));
            }
        }
        //Метод поиска
        void startSearch()
        {
            try
            {
                DeviceData.dataUpdate();
                Dispatcher.Invoke(new Action(() =>
                {
                    searchDef = defecktCheck.IsChecked.Value;
                    searchDel = delCheck.IsChecked.Value;
                    pprDate = dateChB.IsChecked.Value;
                    Exp = expChB.IsChecked.Value;
                    searchStart = expDateStart.SelectedDate != null ? expDateStart.SelectedDate : DateTime.MinValue;
                    searchEnd = expDateEnd.SelectedDate != null ? expDateEnd.SelectedDate : DateTime.MaxValue;
                    dSearch = new List<string>() { searchTBNum.Text, searchTBName.Text};
                }));

                DeviceData.Search(dSearch, objects, searchStart.Value, searchEnd.Value, searchDef, searchDel, pprDate, Exp);

                Dispatcher.Invoke(new Action(() =>
                {
                    BottomUpdate();

                    deviceGrid.ItemsSource = null;
                    deviceGrid.ItemsSource = DeviceData.deviceListMain;

                    pprGrid.ItemsSource = null;
                    pprGrid.ItemsSource = DeviceData.deviceListPPR;

                    excGrid.ItemsSource = null;
                    excGrid.ItemsSource = DeviceData.deviceListExc;

                    if (DeviceData.deviceListMain.Count == 0)
                        MessageBox.Show("Не было найдено приборов по заданым критериям", "Поиск", MessageBoxButton.OK, MessageBoxImage.Information);
                }));
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }));
            }
        }



        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(startSearch);
            thread.Start();
        }

        private void cDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (User.RoleID >= 2)
                {
                    List<Device> devices = new List<Device>();
                    foreach (DeviceData d in deviceGrid.SelectedItems)
                    {
                        devices.Add(context.Device.Where(dev => dev.Device_ID == d.ID).FirstOrDefault());
                    }
                    DeviceData.deviceDel(devices, context, User.Id);
                }
                else MessageBox.Show("Для удаления необходимо иметь роль 'Пользователь' или выше");
                Thread thread = new Thread(UpdateTabs) { IsBackground = true };
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (User.RoleID >= 2)
                {
                    DeviceWin newDevice = new DeviceWin() { User = this.User };
                    newDevice.ShowDialog();
                    switch (newDevice.DialogResult)
                    {
                        case true:
                            MessageBox.Show("Сохранено!", "Добавление", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        case false:
                            MessageBox.Show("Добавление отменено", "Добавление", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        default:

                            break;
                    }
                }
                else MessageBox.Show("Для добавления необходимо иметь роль 'Пользователь' или выше");
                Thread thread = new Thread(UpdateTabs) { IsBackground = true };
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void restoreBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                defecktCheck.IsChecked = false;
                delCheck.IsChecked = false;
                expDateStart.SelectedDate = null;
                expDateEnd.SelectedDate = null;
                searchTBNum.Text = "";
                searchTBName.Text = "";
                searchTBObj.Text = "";
                objects.Clear();
                Thread thread = new Thread(UpdateTabs) { IsBackground = true };
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void redacting()
        {
            try
            {
                if (User.RoleID >= 2)
                {
                    int index = ((DeviceData)deviceGrid.SelectedItem).ID;
                    DeviceWin newDevice = new DeviceWin(false, index) { User = this.User };
                    newDevice.ShowDialog();
                    switch (newDevice.DialogResult)
                    {
                        case true:
                            MessageBox.Show("Сохранено!", "Изменение", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        case false:
                            MessageBox.Show("Изменение отменено", "Изменение", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        default:

                            break;
                    }
                }
                else MessageBox.Show("Для редактирования необходимо иметь роль 'Пользователь' или выше");
                Thread thread = new Thread(UpdateTabs) { IsBackground = true };
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void contextEdit_Click(object sender, RoutedEventArgs e)
        {
            redacting();
        }

        private void redactBtn_Click(object sender, RoutedEventArgs e)
        {
            redacting();
        }

        private void deviceGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                if (User.RoleID >= 2)
                {
                    DeviceData device = e.Row.Item as DeviceData;
                    Device dev = context.Device.Where(d => d.Device_ID == device.ID).FirstOrDefault();
                    DeviceData.DeviceEdit(dev, device.Name, device.ObjectName, device.FNum, device.Param, device.MetrData, device.ExpDate, device.Period.Replace(':', ' '), device.Note.Replace(':', ' '), User.Id);
                    Thread thread = new Thread(UpdateTabs) { IsBackground = true };
                    thread.Start();
                }
                else MessageBox.Show("Для редактирования необходимо иметь роль 'Пользователь' или выше");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        void BottomUpdate()
        {
            try
            {
                switch (mainTab.SelectedIndex)
                {
                    case 0:
                        itemCountLbl.Content = DeviceData.infoMain;

                        checksStack.IsEnabled = true;
                        expChB.IsEnabled = true;
                        DatePickers.IsEnabled = true;
                        dateChB.IsEnabled = false;
                        break;
                    case 1:
                        itemCountLbl.Content = DeviceData.infoPPR;

                        dateChB.IsEnabled = true;
                        checksStack.IsEnabled = false;
                        expChB.IsEnabled = false;
                        DatePickers.IsEnabled = !dateChB.IsChecked.Value;
                        addBtn.IsEnabled = false;
                        break;
                    case 2:
                        itemCountLbl.Content = DeviceData.infoExc;

                        dateChB.IsEnabled = false;
                        checksStack.IsEnabled = false;
                        expChB.IsEnabled = false;
                        DatePickers.IsEnabled = false;
                        addBtn.IsEnabled = false;
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void mainTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BottomUpdate();
        }

        private void dateChB_Checked(object sender, RoutedEventArgs e)
        {
            DatePickers.IsEnabled = !dateChB.IsChecked.Value;
        }

        private void printBtn_Click(object sender, RoutedEventArgs e)
        {
            ExcelExportWindow excelExport = new ExcelExportWindow();
            excelExport.ShowDialog();
        }

        private void userBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (User.RoleID == 3)
                {
                    UserManagmentWindow umw = new UserManagmentWindow(User.Id);
                    umw.ShowDialog();
                }
                else MessageBox.Show("Для данной функции необходимо иметь доступ 'Администратор'");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void journalBtn_Click(object sender, RoutedEventArgs e)
        {
            ActionsWindow actionsWindow = new ActionsWindow();
            actionsWindow.Show();
        }

        private void infoBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Metr - программа предназначенная для ведения журнала приборов, отслеживания их срока годности и составления журнала ППР.\nРазработчик: Асонов Г.С.", "О программе", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void signOutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы хотите выйти из текущей учётной записи?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) logIn();
        }

        private void cRec_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (User.RoleID >= 2)
                {
                    List<Device> devices = new List<Device>();
                    foreach (DeviceData d in excGrid.SelectedItems)
                    {
                        devices.Add(context.Device.Where(dev => dev.Device_ID == d.ID).FirstOrDefault());
                    }
                    DeviceData.deviceRec(devices, context, User.Id);
                }
                else MessageBox.Show("Для восстановления необходимо иметь роль 'Пользователь' или выше");
                Thread thread = new Thread(UpdateTabs) { IsBackground = true };
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void pprGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                if (User.RoleID >= 2)
                {
                    int index = ((DeviceData)pprGrid.SelectedItem).ID;
                    DeviceWin newDevice = new DeviceWin(false, index) { User = this.User };
                    newDevice.ShowDialog();
                    switch (newDevice.DialogResult)
                    {
                        case true:
                            MessageBox.Show("Сохранено!", "Изменение", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        case false:
                            MessageBox.Show("Изменение отменено", "Изменение", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        default:

                            break;
                    }
                }
                else MessageBox.Show("Для редактирования необходимо иметь роль 'Пользователь' или выше");
                Thread thread = new Thread(UpdateTabs) { IsBackground = true };
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void excGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            try
            {
                if (User.RoleID >= 2)
                {
                    int index = ((DeviceData)excGrid.SelectedItem).ID;
                    DeviceWin newDevice = new DeviceWin(false, index) { User = this.User };
                    newDevice.ShowDialog();
                    switch (newDevice.DialogResult)
                    {
                        case true:
                            MessageBox.Show("Сохранено!", "Изменение", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        case false:
                            MessageBox.Show("Изменение отменено", "Изменение", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        default:

                            break;
                    }
                }
                else MessageBox.Show("Для редактирования необходимо иметь роль 'Пользователь' или выше");
                Thread thread = new Thread(UpdateTabs) { IsBackground = true };
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void AddObjBtn_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(searchTBObj.Text) && !objects.Contains(searchTBObj.Text))
            objects.Add(searchTBObj.Text);

            ObjListView.ItemsSource = null;
            ObjListView.ItemsSource = objects;
        }
        private void searchTBObj_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            objects.RemoveAt(ObjListView.SelectedIndex);

            ObjListView.ItemsSource = null;
            ObjListView.ItemsSource = objects;
        }

        private void searchTBObjItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (searchTBObj.SelectedItem != null)
                    if (searchTBObj.SelectedIndex != -1 && !objects.Contains(searchTBObj.Text))
                    {
                        objects.Add(searchTBObj.SelectedItem.ToString());

                        ObjListView.ItemsSource = null;
                        ObjListView.ItemsSource = objects;
                    }
            }
            catch
            {
                searchTBObj.Text = "";
            }
        }
    }
}
