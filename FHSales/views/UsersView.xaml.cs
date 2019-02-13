using FHSales.Classes;
using FHSales.MongoClasses;
using MahApps.Metro.Controls.Dialogs;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FHSales.views
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class UsersView : UserControl
    {

        public UsersView()
        {
            InitializeComponent();

        }

        ConnectionDB conDB;
        MahApps.Metro.Controls.MetroWindow window;
        Users userToUpdate;

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            dgvRegisteredUsers.ItemsSource = await loadDataGridDetails();
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private async Task<List<Users>> loadDataGridDetails()
        {
            List<Users> lstAgnts = new List<Users>();
            conDB = new ConnectionDB();
            try
            {
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Users>("Users");
                var filter = Builders<FHSales.MongoClasses.Users>.Filter.And(
        Builders<Users>.Filter.Where(p => p.isDeleted == false));
                lstAgnts = collection.Find(filter).ToList();
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
            return lstAgnts;
        }

        private async void saveRecord()
        {
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                Users agent = new Users();
                agent.FirstName = txtFirstName.Text;
                agent.LastName = txtLastName.Text;
                agent.Username = txtUsername.Text;
                agent.Password = txtPassword.Password;
                PasswordHash ph = new MongoClasses.PasswordHash(txtPassword.Password);
                agent.bHash = ph.Hash;
                agent.bSalt = ph.Salt;
                agent.isDSAdmin = (chkAdminDS.IsChecked.Value) ? true : false;
                agent.isPOAdmin = (chkAdminPO.IsChecked.Value) ? true : false;

                var collection = db.GetCollection<Users>("Users");
                collection.InsertOne(agent);
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
        }

        private async void updateRecord()
        {
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");

                userToUpdate.FirstName = txtFirstName.Text;
                userToUpdate.LastName = txtLastName.Text;
                userToUpdate.Username = txtUsername.Text;
                userToUpdate.Password = txtPassword.Password;
                PasswordHash ph = new MongoClasses.PasswordHash(txtPassword.Password);
                userToUpdate.bHash = ph.Hash;
                userToUpdate.bSalt = ph.Salt;
                userToUpdate.isDSAdmin = (chkAdminDS.IsChecked.Value) ? true : false;
                userToUpdate.isPOAdmin = (chkAdminPO.IsChecked.Value) ? true : false;

                var filter = Builders<Users>.Filter.And(
            Builders<Users>.Filter.Where(p => p.Id == userToUpdate.Id));
                var updte = Builders<Users>.Update.Set("FirstName", userToUpdate.FirstName)
                    .Set("LastName", userToUpdate.LastName)
                    .Set("Username", userToUpdate.Username)
                    .Set("Password", userToUpdate.Password)
                    .Set("bHash", userToUpdate.bHash)
                    .Set("bSalt", userToUpdate.bSalt)
                    .Set("isDSAdmin", userToUpdate.isDSAdmin)
                    .Set("isPOAdmin", userToUpdate.isPOAdmin);

                var collection = db.GetCollection<Users>("Users");
                collection.UpdateOne(filter, updte);
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
        }

        private void btnAddDirectSales_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEditDirectSales_Click(object sender, RoutedEventArgs e)
        {
            RegisteredUserModel user = dgvRegisteredUsers.SelectedItem as RegisteredUserModel;
            if (user != null)
            {
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;

                txtFirstName.Text = user.Firstname;
                txtLastName.Text = user.Lastname;
                txtUsername.Text = user.Username;
                txtPassword.Password = user.Password;
            }

        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool bl = await checkFields();
            if (bl)
            {
                saveRecord();
                dgvRegisteredUsers.ItemsSource = await loadDataGridDetails();
                dgvRegisteredUsers.Items.Refresh();
                txtFirstName.Text = "";
                txtLastName.Text = "";
                txtUsername.Text = "";
                txtPassword.Password = "";

            }
        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(txtFirstName.Text))
            {
                await window.ShowMessageAsync("FIRST NAME", "Please provide first name.");
            }
            else if (string.IsNullOrEmpty(txtLastName.Text))
            {
                await window.ShowMessageAsync("LAST NAME", "Please provide last name.");
            }
            else if (string.IsNullOrEmpty(txtUsername.Text))
            {
                await window.ShowMessageAsync("USERNAME", "Please provide username.");
            }
            else if (string.IsNullOrEmpty(txtPassword.Password))
            {
                await window.ShowMessageAsync("PASSWORD", "Please provide password.");
            }
            else
            {
                ifAllCorrect = true;
            }

            return ifAllCorrect;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (btnUpdate.Visibility == Visibility.Hidden)
            {
                txtFirstName.Text = "";
                txtLastName.Text = "";
                txtUsername.Text = "";
                txtPassword.Password = "";

            }
            else
            {
                txtFirstName.Text = "";
                txtLastName.Text = "";
                txtUsername.Text = "";
                txtPassword.Password = "";

                btnUpdate.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Visible;
            }
        }


        #region
        //private List<RegisteredUserModel> loadDataGridDetails()
        //{
        //    conDB = new ConnectionDB();
        //    List<RegisteredUserModel> lstUserModel = new List<RegisteredUserModel>();
        //    RegisteredUserModel registeredModel = new RegisteredUserModel();

        //    string queryString = "SELECT dbfh.tblusers.ID, firstname, lastname, username, cast(aes_decrypt(dbfh.tblusers.password, 'pr1nce') as char(100)) as pas, " +
        //        "dbfh.tblusertypes.description as usertype, dbfh.tblusers.usertype as usertypeid FROM (dbfh.tblusers INNER JOIN dbfh.tblusertypes ON dbfh.tblusers.usertype = " +
        //        "dbfh.tblusertypes.ID) WHERE dbfh.tblusers.isDeleted = 0";

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

        //    while (reader.Read())
        //    {
        //        registeredModel.ID = reader["ID"].ToString();
        //        registeredModel.Firstname = reader["firstname"].ToString();
        //        registeredModel.Lastname = reader["lastname"].ToString();
        //        registeredModel.Username = reader["username"].ToString();
        //        registeredModel.UserType = reader["usertype"].ToString();
        //        registeredModel.UserTypeID = reader["usertypeid"].ToString();
        //        registeredModel.Password = reader["pas"].ToString();

        //        lstUserModel.Add(registeredModel);
        //        registeredModel = new RegisteredUserModel();
        //    }

        //    return lstUserModel;
        //}

        //private void loadComboList()
        //{
        //    conDB = new ConnectionDB();
        //    UserTypeModel userTypes = new UserTypeModel();

        //    string queryString = "SELECT ID, type, description FROM dbfh.tblusertypes WHERE isDeleted = 0";

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

        //    while (reader.Read())
        //    {
        //        userTypes.ID = reader["ID"].ToString();
        //        userTypes.Type = reader["type"].ToString();
        //        userTypes.Description = reader["description"].ToString();

        //        cmbActType.Items.Add(userTypes);

        //        userTypes = new UserTypeModel();
        //    }

        //    conDB.closeConnection();
        //}

        //private void loadComboList(RegisteredUserModel regUser)
        //{
        //    conDB = new ConnectionDB();
        //    UserTypeModel userTypes = new UserTypeModel();

        //    string queryString = "SELECT ID, type, description FROM dbfh.tblusertypes WHERE isDeleted = 0";

        //    MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
        //    cmbActType.Items.Clear();
        //    while (reader.Read())
        //    {
        //        userTypes.ID = reader["ID"].ToString();
        //        userTypes.Type = reader["type"].ToString();
        //        userTypes.Description = reader["description"].ToString();

        //        cmbActType.Items.Add(userTypes);

        //        if (regUser.UserTypeID.Equals(userTypes.ID))
        //        {
        //            cmbActType.SelectedItem = userTypes;
        //        }
        //        userTypes = new UserTypeModel();
        //    }

        //    conDB.closeConnection();
        //}

        //private void saveUserRecord()
        //{
        //    conDB = new ConnectionDB();

        //    string queryString = "INSERT INTO dbfh.tblusers (username, password, firstname, lastname, usertype, isDeleted) " +
        //        " VALUES (?, aes_encrypt(?, ?), ?,?,?,0)";

        //    List<string> parameters = new List<string>();
        //    parameters.Add(txtUsername.Text);
        //    parameters.Add(txtPassword.Password);
        //    parameters.Add("pr1nce");
        //    parameters.Add(txtFirstName.Text);
        //    parameters.Add(txtLastName.Text);
        //    parameters.Add(cmbActType.SelectedValue.ToString());

        //    conDB.AddRecordToDatabase(queryString, parameters);
        //    conDB.closeConnection();


        //}
        #endregion
    }
}
