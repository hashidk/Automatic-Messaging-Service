// See https://aka.ms/new-console-template for more information

using Mensajería;

DBConnect db = new("localhost", 3306, "admin", "admin", "empresa");
db.Connect();

List<Mensaje> mensajes = db.Query();
//HttpService hs = new("https://graph.facebook.com/v17.0/115597018100112/messages");
HttpService hs = new("http://localhost:8080");

foreach (Mensaje mensaje in mensajes)
{
    await hs.PostAsync(mensaje.phone); //Enviar mensaje
    db.Delete(mensaje.ID);
}

db.Disconnect();

//using System.Configuration;

//string occupation = ConfigurationManager.AppSettings["occupation"];

//Console.WriteLine(occupation);

