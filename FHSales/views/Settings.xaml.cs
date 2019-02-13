using FHSales.Classes;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FHSales.views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        
        public Settings()
        {
            InitializeComponent();
        }

        UsersView _users = new UsersView();
        Bank _bank = new Bank();
        Courier _courier = new Courier();
        Freebies _freebies = new Freebies();
        ProductsViews _products = new ProductsViews();
        Drugstore _drugstore = new Drugstore();
        SalesOffice _salesOffice = new SalesOffice();
        TollPacker _tollPacker = new TollPacker();
        AreaView _areaView = new AreaView();
        AgentView _agent = new AgentView();

        private void content_Loaded(object sender, RoutedEventArgs e)
        {
            if(!UserModel.isDSAdmin || !UserModel.isPOAdmin)
            {
                menuUsers.IsEnabled = false;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            content.Content = _users;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            content.Content = _bank;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            content.Content = _courier;
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            content.Content = _freebies;
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            content.Content = _products;
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            content.Content = _drugstore;
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            content.Content = _salesOffice;

        }

        private void MenuItem_Click_7(object sender, RoutedEventArgs e)
        {
            content.Content = _tollPacker;
        }

        private void MenuItem_Click_8(object sender, RoutedEventArgs e)
        {
            content.Content = _areaView;
        }

        private void MenuItem_Click_9(object sender, RoutedEventArgs e)
        {
            content.Content = _agent;
        }
    }
}
