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
        }

        protected override void OnStart(string[] args)
        {
            new Logger().WriteToFile("Servicio iniciado el " + DateTime.Now);
            
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 30*this.segundos;
            timer.Enabled = true;

            ExecuteCommand();
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            new Logger().WriteToFile("Servicio ejecutado el " + DateTime.Now);
            ExecuteCommand();
            
        }

        protected override void OnStop()
        {
            new Logger().WriteToFile("Servicio detenido el " + DateTime.Now);
            db.Disconnect();
        }

        protected async void ExecuteCommand()
        {
            db.Connect();
            List<Mensaje> mensajes = db.GetData();
            foreach (Mensaje mensaje in mensajes)
            {
                //await hs.PostAsync(mensaje.phone, mensaje.message); //Enviar mensaje
                new Logger().WriteToFile($"Mensaje enviado a {mensaje.phone} el " + DateTime.Now);
                db.Delete(mensaje.ID);
            }
            db.Disconnect();
        }

    }
}
