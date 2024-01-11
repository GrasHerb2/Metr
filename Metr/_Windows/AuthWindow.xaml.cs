using Metr.Classes;
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
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        public CurrentUser authUser;
        public AuthWindow()
        {
            InitializeComponent();
        }

        void Enter()
        {
            var result = UControl.passwCheck(loginTxt.Text, passTxt.Password);
            switch (result)
            {
                case 2:
                    MessageBox.Show("Данная учётная запись была отключена", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 1:
                    MessageBox.Show("Данная учётная запись ещё не подтверждена", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 0:
                    try
                    {
                        string log = UControl.Sha256Coding(loginTxt.Text);
                        User u0 = MetrBaseEntities.GetContext().User.Where(q => q.ULogin == log).FirstOrDefault();
                        authUser = new CurrentUser() { Id = u0.User_ID, FullName = u0.FullName, RoleID = u0.RoleID };
                        this.DialogResult = true;
                        return;
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Произошла ошибка\n" + ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case -1:
                    MessageBox.Show("Введён неверный пароль", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Stop);
                    
                    break;
                case -2:
                    MessageBox.Show("Пользователь не найден", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Stop);
                    
                    break;
                case -3:
                    MessageBox.Show("Ошибка доступа к БД", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Error);
                    
                    break;
            }
        }

        private void entBtn_Click(object sender, RoutedEventArgs e)
        {
            Enter();            
        }

        private void readBtn_Click(object sender, RoutedEventArgs e)
        {
            authUser = new CurrentUser() { Id = 1, FullName = "Гость", RoleID = 1 };

            this.DialogResult = true;
            return;
        }

        private void regBtn_Click(object sender, RoutedEventArgs e)
        {
            RegWindow a = new RegWindow();
            a.ShowDialog();            
        }
    }
}
