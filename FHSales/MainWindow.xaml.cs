using FHSales.Classes;
using MahApps.Metro.Controls;
using System.Windows;

namespace FHSales
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        LogIn logInWindow;
        //public UserModel user; 

        public MainWindow(LogIn lg)
        {
            logInWindow = lg;
            //user = um;
            InitializeComponent();
        }

        private void HamburgerMenuControl_OnItemClick(object sender, ItemClickEventArgs e)
        {
            this.HamburgerControl.Content = e.ClickedItem;
            this.HamburgerControl.IsPaneOpen = false;
            
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            logInWindow.Show();
            logInWindow.txtUsername.Text = "";
            logInWindow.txtPassword.Password = "";
            UserModel.FirstName = "";
            UserModel.LastName = "";
            UserModel.UserType = "";
            
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserModel.UserType.Equals(UserTypeEnum.DIRECTSALES_VIEW))
            {
                menuSettings.IsEnabled = false;
            }
        }
    }
}
