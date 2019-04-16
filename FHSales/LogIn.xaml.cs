using FHSales.Classes;
using FHSales.MongoClasses;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
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

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {

            if (await verifyLoginMongo())
            {

                MainWindow main = new MainWindow(this);
                main.Show();

                this.Hide();

            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            //conDB = new ConnectionDB();
            //MongoClient client = conDB.initializeMongoDB();
            //var db = client.GetDatabase("DBFH");

            //Users u = new Users();
            //u.FirstName = "Shekinah";
            //u.LastName = "Passion";
            //u.isEditing = true;
            //u.isViewing = true;
            //u.Password = "sp3ctrum";
            //u.Username = "admindsconso";
            //u.isDSAdmin = false;
            //u.isPOAdmin = false;
            //u.isDSConsoAdmin = true;
            //PasswordHash ph = new PasswordHash(u.Password);
            //u.bHash = ph.Hash;
            //u.bSalt = ph.Salt;

            //var collection = db.GetCollection<Users>("Users");
            //collection.InsertOne(u);
        }

        private async System.Threading.Tasks.Task<bool> verifyLoginMongo()
        {
            var z = await this.ShowProgressAsync("LOADING", "Please wait...", false) as ProgressDialogController;
            bool ifCorrect = false;
            PasswordHash ph;
            conDB = new ConnectionDB();
            MongoClient client = conDB.initializeMongoDB();
            var db = client.GetDatabase("DBFH");
            var collection = db.GetCollection<Users>("Users");
            var filter = Builders<Users>.Filter.And(
    Builders<Users>.Filter.Where(p => p.Username.Equals(txtUsername.Text)),
    Builders<Users>.Filter.Where(p => p.isDeleted == false));

            List<Users> lstUser = collection.Find(filter).ToList();

            int x = lstUser.Count;

            if (x > 0)
            {
                foreach (Users u in lstUser)
                {
                    UserModel.FirstName = u.FirstName;
                    UserModel.LastName = u.LastName;
                    UserModel.Username = u.Username;
                    UserModel.Password = u.Password;
                    UserModel.isPOAdmin = u.isPOAdmin;
                    UserModel.isDSAdmin = u.isDSAdmin;
                    UserModel.isDSConsoAdmin = u.isDSConsoAdmin != null ? u.isDSConsoAdmin : false;
                    UserModel.bHash = u.bHash;
                    UserModel.bSalt = u.bSalt;
                    ph = new PasswordHash(u.bSalt, u.bHash);
                    ifCorrect = ph.Verify(txtPassword.Password);
                }
            }
            await z.CloseAsync();
            return ifCorrect;
        }

        //private bool verifyLogin()
        //{
        //    conDB = new ConnectionDB();
        //    //ser = new UserModel();
        //    bool ifCorrect = false;
        //    string queryString = "SELECT dbfh.tblusers.ID, username, aes_decrypt(dbfh.tblusers.password, ?) as pas, firstname, lastname, dbfh.tblusers.usertype " +
        //        "FROM (dbfh.tblusers INNER JOIN dbfh.tblusertypes ON dbfh.tblusers.usertype = dbfh.tblusertypes.ID) " +
        //        "WHERE aes_decrypt(dbfh.tblusers.password, ?) = ? AND username = ? AND dbfh.tblusers.isDeleted = 0";

        //    List<string> parameters = new List<string>();

        //    parameters.Add("pr1nce");
        //    parameters.Add("pr1nce");
        //    parameters.Add(txtPassword.Password);
        //    parameters.Add(txtUsername.Text);

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, parameters);

        //    while (reader.Read())
        //    {
        //        UserModel.ID = reader["ID"].ToString();
        //        UserModel.FirstName = reader["firstname"].ToString();
        //        UserModel.LastName = reader["lastname"].ToString();
        //        UserModel.Username = reader["username"].ToString();
        //        UserModel.UserType = reader["usertype"].ToString();

        //        ifCorrect = true;
        //    }
        //    conDB.closeConnection();

        //    return ifCorrect;
        //}

        private async void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            string filePath = @"C:\Logs\Error.txt";
            try
            {
                if (e.Key == Key.Return && e.Key == Key.Enter)
                {
                    if (await verifyLoginMongo())
                    {
                        MainWindow main = new MainWindow(this);
                        main.Show();
                        this.Hide();
                    }
                    else
                    {
                        await this.ShowMessageAsync("LOG IN", "Incorrect username/password. Please try again!");
                    }
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Message :" + "Login ERROR: " + ex.Message + " " + Environment.NewLine + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }
            }

        }
    }
}
