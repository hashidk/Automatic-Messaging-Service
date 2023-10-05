using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using System.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace AutomaticMessaging
{
    public partial class AutomaticMessagingService : ServiceBase
    {
        Timer timer = new Timer(); //Ojo

        DBConnect db;
        int segundos = 1000;
        HttpService hs;

        public AutomaticMessagingService()
        {
            InitializeComponent();
            string ip_server = ConfigurationManager.AppSettings["IPServer"];
            string PORT = ConfigurationManager.AppSettings["PORT"];
            string username = ConfigurationManager.AppSettings["username"];
            string password = ConfigurationManager.AppSettings["password"];
            string dbname = ConfigurationManager.AppSettings["dbname"];
            this.db = new DBConnect(ip_server, int.Parse(PORT), username, password, dbname);

            string version = ConfigurationManager.AppSettings["APIversion"];
            string phone_id = ConfigurationManager.AppSettings["APIphoneID"];
            this.hs = new HttpService($"https://graph.facebook.com/{version}/{phone_id}/messages");

            var accountSid = "AC37cb81d10e17e5edf8ef63312b14b927";
            var authToken = "4ce9f0bf71ae48419c1c8ca263c1e59c";
            TwilioClient.Init(accountSid, authToken);
        }

        protected override void OnStart(string[] args)
        {
            new Logger().WriteToFile("[INFO] Servicio iniciado el " + DateTime.Now);

            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 30 * this.segundos;
            timer.Enabled = true;

            ExecuteCommand();
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            new Logger().WriteToFile("[INFO] Servicio ejecutado el " + DateTime.Now);
            ExecuteCommand();

        }

        protected override void OnStop()
        {
            new Logger().WriteToFile("[INFO] Servicio detenido el " + DateTime.Now);
            db.Disconnect();
        }

        protected void ExecuteCommand()
        {
            db.Connect();
            string from_phone   = "+14155238886";
            List<Mensaje> mensajes = db.GetData();
            foreach (Mensaje mensaje in mensajes)
            {
                string to_phone = mensaje.phone;
                if (!to_phone[0].Equals("+")) to_phone = "+" + to_phone;

                var messageOptions  = new CreateMessageOptions(new PhoneNumber($"whatsapp:{to_phone}"));
                messageOptions.From = new PhoneNumber($"whatsapp:{from_phone}");
                messageOptions.Body = mensaje.message;
                var message = MessageResource.Create(messageOptions);
                new Logger().WriteToFile("[DEBUG] Mensaje enviado a " + mensaje.phone);
            }
            db.Disconnect();
        }

    }
}
