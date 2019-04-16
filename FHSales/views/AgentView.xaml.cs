using FHSales.Classes;
using FHSales.MongoClasses;
using MahApps.Metro.Controls.Dialogs;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FHSales.views
{
    /// <summary>
    /// Interaction logic for AgentView.xaml
    /// </summary>
    public partial class AgentView : UserControl
    {
        public AgentView()
        {
            InitializeComponent();
        }

        ConnectionDB conDB;
        MahApps.Metro.Controls.MetroWindow window;
        Agents agents = new Agents();
        Agents agentToUpdate;

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
            window = Window.GetWindow(this) as MahApps.Metro.Controls.MetroWindow;
            dgvArea.ItemsSource = await loadDataGridDetails();
        }

        private async Task<List<Agents>> loadDataGridDetails()
        {
            List<Agents> lstAgnts = new List<Agents>();
            conDB = new ConnectionDB();
            try
            {
                MongoClient client = conDB.initializeMongoDB();
                var db = client.GetDatabase("DBFH");
                var collection = db.GetCollection<Agents>("Agents");
                var filter = Builders<Agents>.Filter.And(
        Builders<Agents>.Filter.Where(p => p.isDeleted == false));
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
                Agents agent = new Agents();
                agent.AgentName = txtAgentName.Text;
                var collection = db.GetCollection<Agents>("Agents");
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

                var filter = Builders<Agents>.Filter.And(
            Builders<Agents>.Filter.Where(p => p.Id == agentToUpdate.Id));
                var updte = Builders<Agents>.Update.Set("AgentName", txtAgentName.Text);

                var collection = db.GetCollection<Agents>("Agents");
                collection.UpdateOne(filter, updte);
            }
            catch (Exception ex)
            {
                await window.ShowMessageAsync("ERROR", "Caused by: " + ex.StackTrace);
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAgentName.Text))
            {
                updateRecord();
                clearFields();
                dgvArea.ItemsSource = await loadDataGridDetails();
                await window.ShowMessageAsync("UPDATE RECORD", "Record saved successfully!");
            }
            else
            {
                await window.ShowMessageAsync("AGENT NAME", "Please input value.");
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtAgentName.Text))
            {
                saveRecord();
                clearFields();
                dgvArea.ItemsSource = await loadDataGridDetails();
                await window.ShowMessageAsync("SAVE RECORD", "Record saved successfully!");
            }
            else
            {
                await window.ShowMessageAsync("AGENT NAME", "Please input value.");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            clearFields();
        }

        private void clearFields()
        {
            txtAgentName.Text = "";
            btnUpdate.Visibility = Visibility.Hidden;
            btnSave.Visibility = Visibility.Visible;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            agentToUpdate = dgvArea.SelectedItem as Agents;

            if (agentToUpdate != null)
            {
                txtAgentName.Text = agentToUpdate.AgentName;
                btnSave.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Visible;
            }
        }
    }
}
