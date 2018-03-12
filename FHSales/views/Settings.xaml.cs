﻿using System;
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

        Users _users = new Users();
        Bank _bank = new Bank();
        Courier _courier = new Courier();
        Freebies _freebies = new Freebies();
        Products _products = new Products();
        Drugstore _drugstore = new Drugstore();

        private void content_Loaded(object sender, RoutedEventArgs e)
        {

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
    }
}