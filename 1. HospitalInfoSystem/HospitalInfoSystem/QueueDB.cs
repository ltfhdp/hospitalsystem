using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;



namespace HospitalInfoSystem
{
    class QueueDB
    {
        private static IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "AIzaSyBPAlihF3aUO1ujj4NWQSQs-Ax8a445g0c",
            BasePath = "https://hospitalsystem-b6442-default-rtdb.firebaseio.com"
        };

        private static IFirebaseClient client;

        public static void ConnectDB()
        {
            client = new FireSharp.FirebaseClient(config);
        }

        public static async Task<QueueData> InsertData(QueueData queueData)
        {
            SetResponse response = await client.SetTaskAsync("HospitalQueue/" + queueData.ReservationTime + "/" + queueData.QueueNumber, queueData);
            QueueData result = response.ResultAs<QueueData>();
            return result;
        }

        public static async Task<QueueCounter> InsertCount(QueueCounter count, string ReservationTime)
        {
            SetResponse response = await client.SetTaskAsync("Counter/" + ReservationTime, count);
            QueueCounter result = response.ResultAs<QueueCounter>();
            return result;
        }

        public static async Task<string> CheckQueueNumber(string reservationTime)
        {
            try
            {
                string q = "";

                if (await client.GetTaskAsync("Counter/" + reservationTime) == null)
                {
                    q = "0";
                }
                else
                {
                    FirebaseResponse response = await client.GetTaskAsync("Counter/" + reservationTime);
                    QueueCounter get = response.ResultAs<QueueCounter>();
                    q = get.q;
                }
                return q;
            }
            catch(Exception e)
            {
                return "0";
            }
        }

        public static async Task<QueueData> GetQueueData(string reservationDate, string i)
        {
            FirebaseResponse response = await client.GetTaskAsync("HospitalQueue/" + reservationDate + "/" + i);
            QueueData result = response.ResultAs<QueueData>();

            return result;
        }

        public static async Task<QueueData> ChangeStatus(QueueData queueData)
        {
            FirebaseResponse response = await client.UpdateTaskAsync("HospitalQueue/" + queueData.ReservationTime + "/" + queueData.QueueNumber, queueData);
            QueueData result = response.ResultAs<QueueData>();
            return result;
        }
    }
}
