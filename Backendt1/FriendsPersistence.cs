using Backendt1.DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Backendt1
{


    public class FriendsPersistence
    {
        public string connectionString = "Database=sbadb;Data Source=eu-cdbr-azure-west-a.cloudapp.net;User Id=b8788f67037f24;Password=89de9c53";
        public ArrayList getFriends()
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

                String sqlString = "SELECT * FROM friends";

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);


                mySQLReader = cmd.ExecuteReader();
                while (mySQLReader.Read())
                {
                    Friends f = new Friends();

                    f.UserFBID = mySQLReader.GetString(0);
                    f.FriendFBID = mySQLReader.GetString(1);
                    personArrayL.Add(f);
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

        public ArrayList getFriends(String ID)
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

                //String sqlString = "SELECT FriendFBID FROM friends WHERE UserFBID=" + ID.ToString();
                String order5 =
         "SELECT B.FriendFBID,C.UserName,c.ImgLink FROM friends B INNER JOIN user C on B.FriendFBID = C.FBID WHERE B.UserFBID =" + ID;
                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(order5, conn);


                mySQLReader = cmd.ExecuteReader();
                while (mySQLReader.Read())
                {
                    Friends f = new Friends();
                    f.FriendFBID = mySQLReader.GetString(0);
                    f.UserName = mySQLReader.GetString(1);
                    f.ImgLink = mySQLReader.GetString(2);
                    personArrayL.Add(f);
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

        /*public ArrayList getFriends(long ID)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            string myConnectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString;
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            try
            {
                conn.ConnectionString = myConnectionString;
                conn.Open();
                ArrayList personArrayL = new ArrayList();
                MySql.Data.MySqlClient.MySqlDataReader mySQLReader = null;

                String sqlString = "SELECT FriendFBID FROM friends WHERE UserFBID=" + ID;

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);


                mySQLReader = cmd.ExecuteReader();
                while (mySQLReader.Read())
                {
                    Friends f = new Friends();
                    f.FriendFBID = mySQLReader.GetInt64(0);
                    personArrayL.Add(f);
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
            

        }*/

        public String saveFriend(Friends personToSave)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            //string myConnectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString;
            string myConnectionString = connectionString;
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            try
            {
                conn.ConnectionString = myConnectionString;
                conn.Open();
                String sqlString = "INSERT INTO Friends (UserFBID, FriendFBID)"
                    + "VALUES('" + personToSave.UserFBID + "','" + personToSave.FriendFBID + "');";

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);
                cmd.ExecuteNonQuery();


                return personToSave.UserFBID;
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

        public bool deleteFriend(string userFBID, string friendFBID)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            //string myConnectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString;
            string myConnectionString = connectionString;
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            try
            {
                conn.ConnectionString = myConnectionString;
                conn.Open();

                Friends p = new Friends();
                MySql.Data.MySqlClient.MySqlDataReader mySQLReader = null;

                String sqlString = "SELECT UserFBID, FriendFBID FROM friends WHERE UserFBID=" + userFBID + " AND FriendFBID =" + friendFBID;

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);


                mySQLReader = cmd.ExecuteReader();
                if (mySQLReader.Read())
                {
                    mySQLReader.Close();

                    sqlString = "DELETE FROM friends WHERE UserFBID=" + userFBID + " AND FriendFBID =" + friendFBID;

                    cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);

                    cmd.ExecuteNonQuery();
                    return true;
                }
                else
                {
                    return false;
                }
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


        public bool updateFriend(string ID, Friends personToSave)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            //string myConnectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString;
            string myConnectionString = connectionString;
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            try
            {
                conn.ConnectionString = myConnectionString;
                conn.Open();
                MySql.Data.MySqlClient.MySqlDataReader mySQLReader = null;

                String sqlString = "SELECT * FROM friends WHERE UserFBID =" + ID.ToString();

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);


                mySQLReader = cmd.ExecuteReader();
                if (mySQLReader.Read())
                {
                    mySQLReader.Close();

                    sqlString = "UPDATE friends SET UserFBID=" + personToSave.UserFBID + ", FriendFBID=" + personToSave.FriendFBID
                                                              + "'WHERE UserFBID =" + ID;

                    cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);

                    cmd.ExecuteNonQuery();
                    return true;
                }
                else
                {
                    return false;
                }
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