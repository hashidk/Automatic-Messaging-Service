using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;
using System;
using System.Data.SqlClient;

namespace AutomaticMessaging
{
    struct Mensaje
    {
        public int ID;
        public string codecountry;
        public string phone;
        public string message;

        public Mensaje(int ID, string codecountry, string phone, string message)
        {
            this.ID = ID;
            this.codecountry = codecountry;
            this.phone = phone;
            this.message = message;
        }

        public override string ToString() => $"({ID}, {codecountry}, {phone}, {message})";
    }

    internal class DBConnect
    {
        public MySqlConnection sqlConexion;
        private string IP, username, password, dbname;
        private int port;

        public DBConnect(string IP, int port, string username, string password, string dbname)
        {
            this.sqlConexion = null;
            this.IP = IP;
            this.port = port;
            this.username = username;
            this.password = password;
            this.dbname = dbname;
        }
        public void Connect()
        {
            this.sqlConexion = new MySqlConnection("datasource=" + this.IP + ";port=" + this.port + ";username=" + this.username + ";password=" + this.password + ";database=" + this.dbname);
            try
            {
                this.sqlConexion.Open();
                new Logger().WriteToFile("[DEBUG] Conexión con MySQL iniciada el " + DateTime.Now);
            }
            catch (System.Exception ex)
            { new Logger().WriteToFile("[ERROR] Algo sucedió con MySQL el " + DateTime.Now + "\n" + ex + "\n\n"); }
        }

        public List<Mensaje> GetData()
        {
            List<Mensaje> mensajes = new List<Mensaje> { };

            try
            {
                using (MySqlCommand cmd = this.sqlConexion.CreateCommand())
                {
                    cmd.CommandText = "SELECT * from mensajes";
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            Mensaje m;
                            m.ID = reader.GetInt32("ID");
                            m.codecountry = reader.IsDBNull(1) ? "" : reader.GetString("codecountry");
                            m.phone = reader.GetString("phone");
                            m.message = reader.GetString("message");
                            mensajes.Add(m);
                        }
                }
            }
            catch (Exception e)
            {
                new Logger().WriteToFile("[ERROR] Ocurrió algún error: " + e.Message);
            }
            return mensajes;
        }

        public void Delete(int ID)
        {
            try
            {
                using (MySqlCommand cmd = this.sqlConexion.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM mensajes WHERE ID = @word";
                    cmd.Parameters.AddWithValue("@word", ID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                new Logger().WriteToFile("[ERROR] No se pudo eliminar el registro: " + e.Message);
            }
        }

        public void Disconnect()
        {
            this.sqlConexion.Close();
            new Logger().WriteToFile("[DEBUG] Conexión con MySQL cerrada el " + DateTime.Now);
        }
    }

}

