using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;
using System.Globalization;
using System.Threading;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace HospitalInfoSystem
{
    /// <summary>
    /// Interaction logic for DisplayQueue.xaml
    /// </summary>
    public partial class DisplayQueue : Window
    {
        private string queueDate { get; set; }
        private DataTable dt = new DataTable();

        public DisplayQueue()
        {
            InitializeComponent();
            btnDone.IsEnabled = false;

            dt.Columns.Add("Reservation Date");
            dt.Columns.Add("Queue Number");
            dt.Columns.Add("BPJS");
            dt.Columns.Add("Name");
            dt.Columns.Add("Poli");
            dt.Columns.Add("Status");

            dgQueue.ItemsSource = dt.DefaultView;
        }


        private async void BindingData(string queueDate)
        {
            int queueTotal = Int32.Parse(await QueueDB.CheckQueueNumber(queueDate));

            for (int i=1; i<=queueTotal; i++)
            {
                QueueData obj = new QueueData();
                obj = await QueueDB.GetQueueData(queueDate, i.ToString());

                DataRow row = dt.NewRow();
                row["Reservation Date"] = obj.ReservationTime;
                row["Queue Number"] = obj.QueueNumber;
                row["BPJS"] = obj.BPJS;
                row["Name"] = obj.Name;
                row["Poli"] = obj.Poli;
                row["Status"] = obj.Status;

                dt.Rows.Add(row);
            }

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BtnDone_Click(object sender, RoutedEventArgs e)
        {
            QueueData queueData = new QueueData
            {
                ReservationTime = ((DataRowView)dgQueue.SelectedItem).Row[0].ToString(),
                QueueNumber = ((DataRowView)dgQueue.SelectedItem).Row[1].ToString(),
                BPJS = ((DataRowView)dgQueue.SelectedItem).Row[2].ToString(),
                Name = ((DataRowView)dgQueue.SelectedItem).Row[3].ToString(),
                Poli = ((DataRowView)dgQueue.SelectedItem).Row[4].ToString(),
                Status = "Done"
            };

            QueueData updateResult = await QueueDB.ChangeStatus(queueData);
            MessageBox.Show(string.Format("Patient {0} , queue number {1}, is done.", updateResult.Name, updateResult.QueueNumber));
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            dt.Clear();
            btnDone.IsEnabled = false;
            dgQueue.SelectedItem = null;
            BindingData(queueDate);
        }

        private void DpQueueDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL");
            queueDate = ((DateTime)dpQueueDate.SelectedDate).ToShortDateString();

            dt.Clear();
            BindingData(queueDate);
        }
        private void DgQueue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (((DataRowView)dgQueue.SelectedItem).Row[5].ToString() == "Queueing")
                {
                    btnDone.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
