﻿using FHSales.MongoClasses;
using FHSales.views;
using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Windows;

namespace FHSales
{
    /// <summary>
    /// Interaction logic for AddSubArea.xaml
    /// </summary>
    public partial class AddSubArea : MetroWindow
    {
        public AddSubArea()
        {
            InitializeComponent();
        }

        List<SubArea> lstSubArea = new List<SubArea>();
        AreaView areaView;

        public AddSubArea(AreaView av, List<SubArea> lstAS)
        {
            areaView = av;
            lstSubArea = lstAS;
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dgvSubArea.ItemsSource = lstSubArea;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            SubArea sa = new SubArea();

            sa.SubAreaName = txtSubName.Text;

            lstSubArea.Add(sa);
            dgvSubArea.ItemsSource = lstSubArea;
            dgvSubArea.Items.Refresh();
            txtSubName.Text = "";
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            SubArea sa = dgvSubArea.SelectedItem as SubArea;

            if (sa != null)
            {
                lstSubArea.Remove(sa);
                dgvSubArea.ItemsSource = lstSubArea;
                dgvSubArea.Items.Refresh();
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (areaView != null)
            {
                areaView.lstSubArea = lstSubArea;
            }
        }
    }
}
