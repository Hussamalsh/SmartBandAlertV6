using Backendt1.DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Backendt1
{
    public class VictimPersistence
    {

        public string connectionString = "Database=sbadb;Data Source=eu-cdbr-azure-west-a.cloudapp.net;User Id=b8788f67037f24;Password=89de9c53";


        public String saveUser(Victim personToSave)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            //string myConnectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString;
            string myConnectionString = connectionString;
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            try
            {
                conn.ConnectionString = myConnectionString;
                conn.Open();
                String sqlString = "INSERT INTO viktim (FBID, UserName, StartDate, Latitude, Longitude, Adress)"
                    + "VALUES(" + personToSave.FBID + ",'" + personToSave.UserName + "','" + personToSave.StartDate.ToString("yyyy-MM-dd HH:mm:ss") 
                    + "','" + personToSave.Latitude + "','"+ personToSave.Longitude + "','" + personToSave.Adress + "');";

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);
                cmd.ExecuteNonQuery();

                //string fbid = cmd.LastInsertedId;
                return personToSave.FBID;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

        }



        public ArrayList getVictims()
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            //string myConnectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString;
            string myConnectionString = connectionString;
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            try
            {
                conn.ConnectionString = myConnectionString;
                conn.Open();

                ArrayList personArrayL = new ArrayList();

                MySql.Data.MySqlClient.MySqlDataReader mySQLReader = null;

                String sqlString = "SELECT * FROM viktim";

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);


                mySQLReader = cmd.ExecuteReader();
                while (mySQLReader.Read())
                {
                    Victim u = new Victim();

                    u.FBID = mySQLReader.GetString(0);
                    u.UserName = mySQLReader.GetString(1);
                    u.StartDate = mySQLReader.GetDateTime(2);
                    u.Latitude = mySQLReader.GetString(3);
                    u.Longitude = mySQLReader.GetString(4);
                    u.Adress = mySQLReader.GetString(5);
                    personArrayL.Add(u);
                }


                return personArrayL;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }



        public Victim getVictim(string ID)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            //string myConnectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString;
            string myConnectionString = connectionString;
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            try
            {
                conn.ConnectionString = myConnectionString;
                conn.Open();
                Victim u = new Victim();
                MySql.Data.MySqlClient.MySqlDataReader mySQLReader = null;

                String sqlString = "SELECT * FROM viktim WHERE FBID =" + ID + " ORDER BY startDate DESC LIMIT 1";

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);


                mySQLReader = cmd.ExecuteReader();
                if (mySQLReader.Read())
                {

                    u.FBID = mySQLReader.GetString(0);
                    u.UserName = mySQLReader.GetString(1);
                    u.StartDate = mySQLReader.GetDateTime(2);
                    u.Latitude = mySQLReader.GetString(3);
                    u.Longitude = mySQLReader.GetString(4);
                    u.Adress = mySQLReader.GetString(5);                  

                }
                else
                {
                    return null;
                }

                return u;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

        }




    }
}