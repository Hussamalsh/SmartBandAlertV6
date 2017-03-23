using Backendt1.DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Backendt1
{
    public class UserPersistence
    {
        public string connectionString = "Database=sbadb;Data Source=eu-cdbr-azure-west-a.cloudapp.net;User Id=b8788f67037f24;Password=89de9c53";
        public ArrayList getUsers()
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

                String sqlString = "SELECT * FROM user";

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);


                mySQLReader = cmd.ExecuteReader();
                while (mySQLReader.Read())
                {
                    User u = new User();

                    u.FBID = mySQLReader.GetString(0);
                    u.UserName = mySQLReader.GetString(1);
                    u.Email = mySQLReader.GetString(2);
                    u.ImgLink = mySQLReader.GetString(3);
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



        public User getUser(string ID)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            //string myConnectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString;
            string myConnectionString = connectionString;
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            try
            {
                conn.ConnectionString = myConnectionString;
                conn.Open();
                User u = new User();
                MySql.Data.MySqlClient.MySqlDataReader mySQLReader = null;

                String sqlString = "SELECT * FROM user WHERE FBID =" + ID;

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);


                mySQLReader = cmd.ExecuteReader();
                if (mySQLReader.Read())
                {

                    u.FBID = mySQLReader.GetString(0);
                    u.UserName = mySQLReader.GetString(1);
                    u.Email = mySQLReader.GetString(2);
                    u.ImgLink = mySQLReader.GetString(3);

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



        public ArrayList searchUser(string keyword)
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

                String sqlString = "SELECT * FROM user WHERE MATCH(UserName) AGAINST ('" + keyword + "*' IN BOOLEAN MODE);";

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);


                mySQLReader = cmd.ExecuteReader();
                while (mySQLReader.Read())
                {
                    User u = new User();

                    u.FBID = mySQLReader.GetString(0);
                    u.UserName = mySQLReader.GetString(1);
                    u.Email = mySQLReader.GetString(2);
                    u.ImgLink = mySQLReader.GetString(3);
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




        public String saveUser(User personToSave)
        {
            MySql.Data.MySqlClient.MySqlConnection conn;
            //string myConnectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString;
            string myConnectionString = connectionString;
            conn = new MySql.Data.MySqlClient.MySqlConnection();
            try
            {
                conn.ConnectionString = myConnectionString;
                conn.Open();
                String sqlString = "INSERT INTO user (FBID, UserName, Email, ImgLink)"
                    + "VALUES(" + personToSave.FBID + ",'" + personToSave.UserName + "','" + personToSave.Email + "','" + personToSave.ImgLink + "');";

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);
                cmd.ExecuteNonQuery();

                //long fbid = cmd.LastInsertedId;
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


        public bool deleteUser(String ID)
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

                String sqlString = "SELECT * FROM user WHERE FBID =" + ID.ToString();

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);


                mySQLReader = cmd.ExecuteReader();
                if (mySQLReader.Read())
                {
                    mySQLReader.Close();

                    sqlString = "DELETE FROM user WHERE FBID =" + ID.ToString();

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


        public bool updateUser(string ID, User personToSave)
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

                String sqlString = "SELECT * FROM user WHERE FBID =" + ID.ToString();

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sqlString, conn);


                mySQLReader = cmd.ExecuteReader();
                if (mySQLReader.Read())
                {
                    mySQLReader.Close();

                    sqlString = "UPDATE user SET UserName='" + personToSave.UserName + "', Email='" + personToSave.Email +
                                                              "', ImgLink='" + personToSave.ImgLink + "'WHERE FBID =" + ID.ToString();

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