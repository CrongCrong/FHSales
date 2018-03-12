using FHSales.Classes;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FHSales.views
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : UserControl
    {
        
        public Users()
        {
            InitializeComponent();
           
        }

        ConnectionDB conDB;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadComboList();
            dgvRegisteredUsers.ItemsSource = loadDataGridDetails();
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private List<RegisteredUserModel> loadDataGridDetails()
        {
            conDB = new ConnectionDB();
            List<RegisteredUserModel> lstUserModel = new List<RegisteredUserModel>();
            RegisteredUserModel registeredModel = new RegisteredUserModel();

            string queryString = "SELECT dbfh.tblusers.ID, firstname, lastname, username, cast(aes_decrypt(dbfh.tblusers.password, 'pr1nce') as char(100)) as pas, " +
                "dbfh.tblusertypes.description as usertype, dbfh.tblusers.usertype as usertypeid FROM (dbfh.tblusers INNER JOIN dbfh.tblusertypes ON dbfh.tblusers.usertype = " +
                "dbfh.tblusertypes.ID) WHERE dbfh.tblusers.isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                registeredModel.ID = reader["ID"].ToString();
                registeredModel.Firstname = reader["firstname"].ToString();
                registeredModel.Lastname = reader["lastname"].ToString();
                registeredModel.Username = reader["username"].ToString();
                registeredModel.UserType = reader["usertype"].ToString();
                registeredModel.UserTypeID = reader["usertypeid"].ToString();
                registeredModel.Password = reader["pas"].ToString();

                lstUserModel.Add(registeredModel);
                registeredModel = new RegisteredUserModel();
            }

            return lstUserModel;
        }

        private void loadComboList()
        {
            conDB = new ConnectionDB();
            UserTypeModel userTypes = new UserTypeModel();

            string queryString = "SELECT ID, type, description FROM dbfh.tblusertypes WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);

            while (reader.Read())
            {
                userTypes.ID = reader["ID"].ToString();
                userTypes.Type = reader["type"].ToString();
                userTypes.Description = reader["description"].ToString();

                cmbActType.Items.Add(userTypes);

                userTypes = new UserTypeModel();
            }

            conDB.closeConnection();
        }

        private void loadComboList(RegisteredUserModel regUser)
        {
            conDB = new ConnectionDB();
            UserTypeModel userTypes = new UserTypeModel();

            string queryString = "SELECT ID, type, description FROM dbfh.tblusertypes WHERE isDeleted = 0";

            MySqlDataReader reader = conDB.getSelectConnection(queryString, null);
            cmbActType.Items.Clear();
            while (reader.Read())
            {
                userTypes.ID = reader["ID"].ToString();
                userTypes.Type = reader["type"].ToString();
                userTypes.Description = reader["description"].ToString();

                cmbActType.Items.Add(userTypes);

                if (regUser.UserTypeID.Equals(userTypes.ID))
                {
                    cmbActType.SelectedItem = userTypes;
                }
                userTypes = new UserTypeModel();
            }

            conDB.closeConnection();
        }

        private void btnAddDirectSales_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEditDirectSales_Click(object sender, RoutedEventArgs e)
        {
            RegisteredUserModel user = dgvRegisteredUsers.SelectedItem as RegisteredUserModel;
            if(user != null)
            {
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;

                txtFirstName.Text = user.Firstname;
                txtLastName.Text = user.Lastname;
                txtUsername.Text = user.Username;
                txtPassword.Password = user.Password;
                loadComboList(user);
            }

        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool bl = await checkFields();
            if (bl)
            {
                saveUserRecord();
                dgvRegisteredUsers.ItemsSource = loadDataGridDetails();
                dgvRegisteredUsers.Items.Refresh();
                txtFirstName.Text = "";
                txtLastName.Text = "";
                txtUsername.Text = "";
                txtPassword.Password = "";

            }
        }

        private void saveUserRecord()
        {
            conDB = new ConnectionDB();

            string queryString = "INSERT INTO dbfh.tblusers (username, password, firstname, lastname, usertype, isDeleted) " +
                " VALUES (?, aes_encrypt(?, ?), ?,?,?,0)";

            List<string> parameters = new List<string>();
            parameters.Add(txtUsername.Text);
            parameters.Add(txtPassword.Password);
            parameters.Add("pr1nce");
            parameters.Add(txtFirstName.Text);
            parameters.Add(txtLastName.Text);
            parameters.Add(cmbActType.SelectedValue.ToString());

            conDB.AddRecordToDatabase(queryString, parameters);
            conDB.closeConnection();


        }

        private async Task<bool> checkFields()
        {
            bool ifAllCorrect = false;
            MahApps.Metro.Controls.MetroWindow window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;

            if (string.IsNullOrEmpty(txtFirstName.Text))
            {
                await window.ShowMessageAsync("FIRST NAME", "Please provide first name.");
            }else if (string.IsNullOrEmpty(txtLastName.Text))
            {
                await window.ShowMessageAsync("LAST NAME", "Please provide last name.");
            }else if (string.IsNullOrEmpty(txtUsername.Text))
            {
                await window.ShowMessageAsync("USERNAME", "Please provide username.");
            }else if (string.IsNullOrEmpty(txtPassword.Password))
            {
                await window.ShowMessageAsync("PASSWORD", "Please provide password.");
            }else if(cmbActType.SelectedItem == null)
            {
                await window.ShowMessageAsync("ACCOUNT TYPE", "Please select account type.");
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
            if(btnUpdate.Visibility == Visibility.Hidden)
            {
                txtFirstName.Text = "";
                txtLastName.Text = "";
                txtUsername.Text = "";
                txtPassword.Password = "";

            }else
            {
                txtFirstName.Text = "";
                txtLastName.Text = "";
                txtUsername.Text = "";
                txtPassword.Password = "";

                btnUpdate.Visibility = Visibility.Hidden;
                btnSave.Visibility = Visibility.Visible;
            }
        }
    }
}
