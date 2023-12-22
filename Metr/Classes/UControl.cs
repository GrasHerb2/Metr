using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Documents;

namespace Metr.Classes
{
    internal class UControl
    {
        static MetrBaseEntities context = MetrBaseEntities.GetContext();
        /// <summary>
        /// Кодирование строки в хеш-код SHA256
        /// </summary>
        /// <param name="line">Входящая строка для кодирования</param>
        /// <returns>Хеш-код SHA256</returns>
        public static string Sha256Coding(string line)
        {
            var linecode = SHA256.Create();
            StringBuilder builder = new StringBuilder();
            byte[] bytes = linecode.ComputeHash(Encoding.UTF8.GetBytes(line));
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            line = builder.ToString();
            return line;
        }
        /// <summary>
        /// Метод авторизации
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="passw">Пароль пользователя</param>
        /// <returns>
        /// Коды результата проверок:
        /// 2   Учетная запись отключена;
        /// 1   Учетная запись не подтверждена;
        /// 0   Успешный вход;
        /// -1  Несовпадение пароля;
        /// -2  Отсутсвие учётной записи с указаным логином;
        /// -3  Невозможность получения данных из базы;
        /// </returns>
        public static int passwCheck(string login, string passw)
        {
            try
            {
                var log256 = Sha256Coding(login);

                

                if (context.User.Where(p => p.ULogin == log256 || p.ULogin == "---" + log256).ToList().Count() == 0) return -2;

                var passwCheck = context.User.Where(p => p.ULogin == log256 || p.ULogin == "---" + log256).FirstOrDefault();


                if (passwCheck.UPass.Remove(3).Contains("---") && Sha256Coding(passw) == "---"+passwCheck.UPass) return 1;

                if (passwCheck.ULogin.Remove(3).Contains("---") && Sha256Coding(login) == "---" + passwCheck.ULogin) return 2;

                passw = Sha256Coding(passw);/*входящий пароль кодируется*/
                if (passw == passwCheck.UPass /*пароль в базе*/ )
                    return 0;//вход разрешён
                else return -1;//пароль не совпадает
            }
            catch { return -3; } //код -3 возникает при невозможности подключения к БД
        }
        /// <summary>
        /// Создание нового пользователя
        /// </summary>
        /// <param name="newLogin">Логин нового пользователя</param>
        /// <param name="newPass">Пароль нового пользователя</param>
        /// <param name="newFullName">ФИО нового пользователя</param>
        /// <param name="newMail">Электронная почта нового пользователя (опционально)</param>
        /// <returns>Возвращает класс tResult который содержит объект нового пользователя, запись в журнал аудита и код операции:   1=Пользователь с таким логином уже записан в БД;    0=Операця успешна   -1=Ошибка доступа к БД или иное</returns>
        public static tResult newEmployee(string newLogin, string newPass, string newFullName, string newMail)
        {
            tResult result = new tResult();
            try
            {
                var a = Sha256Coding(newLogin);
                if (context.User.Where(p => p.ULogin == a).Count() > 0)
                {
                    result.resultid = 1;
                    return result;
                }//в системе не может быть двух однинаковых логинов

                result.Action = new Actions()
                {
                    ActionDate = DateTime.Now,
                    UserID = 1,
                    ActionText = "Запрос на регистрацию*компьютер:" + Environment.MachineName.ToString() + "*ФИО:" + newFullName,
                    ComputerName = Environment.MachineName.ToString()
                };
                newLogin = Sha256Coding(newLogin);
                newPass = Sha256Coding(newPass);
                result.User = new User()
                {
                    FullName = newFullName,
                    ULogin = newLogin,
                    RoleID = 1,//неподтверждённый пользователь может использовать только просмотр
                    Email = newMail,
                    UPass = "---" + newPass
                };

                result.resultid = 0;

                return result;

            }
            catch (System.Exception ex)
            {
                result.resultid = -1;
                result.Action = new Actions() { ActionText = ex.Message.ToString() };
                return result;
            }
        }
        /// <summary>
        /// Деактивация учётной записи пользователя
        /// </summary>
        /// <param name="delLogin">Логин отключаемой учётной записи</param>
        /// <param name="admLogin">Логин учётной записи диактивирующего</param>
        /// <returns>Возвращает объект класса tResult хранящий объект отключенной учётной записи, запись  журнал аудита и код операции: 0=Операция успешна  -1=Ошибка отключения
        public static tResult deactiveEmp(string delLogin, string admLogin)
        {
            tResult result = new tResult();
            try
            {
                User deluser = context.User.Where(p => p.ULogin == Sha256Coding(delLogin)).FirstOrDefault();
                User admuser = context.User.Where(p => p.ULogin == Sha256Coding(admLogin)).FirstOrDefault();
                deluser.ULogin = "---" + deluser.ULogin;

                result.User = deluser;
                result.Action = new Actions()
                {
                    UserID = admuser.User_ID,
                    ActionDate = DateTime.Now,
                    ComputerName=Environment.MachineName.ToString(),
                    ActionText= admuser.FullName+" отключил учётную запись "+delLogin
                };
                result.resultid = 0;
                
                return result;
            }
            catch {
                result.resultid = -1;
                return result; }
        }
        public class tResult
        {
            public int resultid { get; set; }
            public User User { get; set; }
            public Actions Action { get; set; }
        }
    }
}

