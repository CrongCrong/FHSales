using FHSales.Classes;
using FHSales.MongoClasses;
using MahApps.Metro.Controls.Dialogs;
using MongoDB.Driver;
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
    /// Interaction logic for AreaView.xaml
    /// </summary>
    public partial class AreaView : UserControl
    {
        public AreaView()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        MahApps.Metro.Controls.MetroWindow window;
        Area area = new Area();
        SubArea subArea = new SubArea();
        public List<SubArea> lstSubArea = new List<SubArea>();
        Area areaToUpdate = new Area();

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            dgvArea.ItemsSource = await loadDataGridDetails();
        }

        private async Task<List<Area>> loadDataGridDetails()
        {
            List<Area> lstDrgs = new List<Area>();
            conDB = new ConnectionDB();
            try
            {
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Area>("Area");
                var filter = Builders<Area>.Filter.And(
        Builders<Area>.Filter.Where(p => p.isDeleted == false));
                lstDrgs = collection.Find(filter).ToList();
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Cause by: " + ex.StackTrace);
            }
            return lstDrgs;
        }

        private async void saveRecord()
        {
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                Area area = new Area();
                area.AreaName = txtAreaName.Text;
                area.SubArea = lstSubArea;
                var collection = db.GetCollection<Area>("Area");
                collection.InsertOne(area);
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Cause by: " + ex.StackTrace);
            }
        }

        private async void updateRecord()
        {
            try
            {
                conDB = new ConnectionDB();
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");

                areaToUpdate.AreaName = txtAreaName.Text;
                areaToUpdate.SubArea = lstSubArea;

                var filter = Builders<Area>.Filter.And(
            Builders<Area>.Filter.Where(p => p.Id == areaToUpdate.Id));
                var updte = Builders<Area>.Update.Set("AreaName", areaToUpdate.AreaName)
                    .Set("SubArea", areaToUpdate.SubArea);

                var collection = db.GetCollection<Area>("Area");
                collection.UpdateOne(filter, updte);


            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Cause by: " + ex.StackTrace);
            }
        }

        private void btnAddSub_Click(object sender, RoutedEventArgs e)
        {
            AddSubArea addSub = new AddSubArea(this, lstSubArea);
            addSub.ShowDialog();
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAreaName.Text))
            {
                updateRecord();
                clearFields();
                dgvArea.ItemsSource = await loadDataGridDetails();
                await window.ShowMessageAsync("UPDATE RECORD", "Record updated successfully!");
            }
            else
            {
                await window.ShowMessageAsync("AREA NAME", "Please input value!");
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAreaName.Text))
            {
                saveRecord();
                clearFields();
                dgvArea.ItemsSource = await loadDataGridDetails();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
            }
            else
            {
                await window.ShowMessageAsync("AREA NAME", "Please input value!");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            clearFields();   
        }

        private void clearFields()
        {
            txtAreaName.Text = "";
            lstSubArea = new List<SubArea>();
            btnSave.Visibility = Visibility.Visible;
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            areaToUpdate = dgvArea.SelectedItem as Area;

            if (areaToUpdate != null)
            {
                txtAreaName.Text = areaToUpdate.AreaName;
                lstSubArea = areaToUpdate.SubArea;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
        }
    }
}
