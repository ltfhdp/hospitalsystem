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
using System.Windows.Shapes;
using System.Globalization;
using System.Threading;

using FireSharp.Response;

namespace HospitalInfoSystem
{
    /// <summary>
    /// Interaction logic for AddQueue.xaml
    /// </summary>
    public partial class AddQueue : Window
    {
        public AddQueue()
        {
            InitializeComponent();
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            //change culture and change date into string
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL");
            string reservationTime = ((DateTime)dpReservationTime.SelectedDate).ToShortDateString();

            //check queue number for that particular date
            string queueCounter = await QueueDB.CheckQueueNumber(reservationTime);

            //DEBUG
            //MessageBox.Show(string.Format("{0}", queueCounter));

            //create new queue
            QueueData queueData = new QueueData
            {
                QueueNumber = (Int32.Parse(queueCounter)+1).ToString(),
                BPJS = tbBPJS.Text,
                Name = tbName.Text,
                Poli = tbPoli.Text,
                ReservationTime  = ((DateTime)dpReservationTime.SelectedDate).ToShortDateString(),
                Status = "Queueing"
            };

            //add queue to db
            QueueData result = new QueueData();
            result = await QueueDB.InsertData(queueData);

            //create new counter
            QueueCounter counter = new QueueCounter
            {
                q = queueData.QueueNumber
            };

            //add counter to db
            QueueCounter addCount = new QueueCounter();
            addCount = await QueueDB.InsertCount(counter, queueData.ReservationTime.ToString());

            //show confirmation and close window
            MessageBox.Show(string.Format("Queue added for {0} with BPJS number {1}, queue number {2}", result.Name, result.BPJS, addCount.q));
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
