using FHSales.Classes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace FHSales
{
    /// <summary>
    /// Interaction logic for LogIn.xaml
    /// </summary>
    public partial class LogIn : MetroWindow
    {
        public LogIn()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (verifyLogin())
            {
                MainWindow main = new MainWindow(this);
                main.Show();
                this.Hide();
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private bool verifyLogin()
        {
            conDB = new ConnectionDB();
           //ser = new UserModel();
            bool ifCorrect = false;
            string queryString = "SELECT dbfh.tblusers.ID, username, aes_decrypt(dbfh.tblusers.password, ?) as pas, firstname, lastname, dbfh.tblusertypes.type AS usertype " +
                "FROM (dbfh.tblusers INNER JOIN dbfh.tblusertypes ON dbfh.tblusers.usertype = dbfh.tblusertypes.ID) " +
                "WHERE aes_decrypt(dbfh.tblusers.password, ?) = ? AND username = ? AND dbfh.tblusers.isDeleted = 0";

            List<string> parameters = new List<string>();

            parameters.Add("pr1nce");
            parameters.Add("pr1nce");
            parameters.Add(txtPassword.Password);
            parameters.Add(txtUsername.Text);

            MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

            while (reader.Read())
            {
                UserModel.ID = reader["ID"].ToString();
                UserModel.FirstName = reader["firstname"].ToString();
                UserModel.LastName = reader["lastname"].ToString();
                UserModel.Username = reader["username"].ToString();
                UserModel.UserType = reader["usertype"].ToString();

                ifCorrect = true;
            }
            conDB.closeConnection();

            return ifCorrect;
        }

        private async void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && e.Key == Key.Enter)
            {
                if (verifyLogin())
                {
                    MainWindow main = new MainWindow(this);
                    main.Show();
                    this.Hide();
                }
                else
                {
                    await this.ShowMessageAsync("LOG IN","Incorrect username/password. Please try again!");
                }
            }
        }
    }
}
