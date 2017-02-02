using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace DBLogger
{
    public class DBManagement
    {
        public SqlConnection DBconnect()
        {
            try
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = "Data Source=localhost;Initial Catalog=EchoBot;Integrated Security=True";
                conn.Open();
                return conn;
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }

        }
        public void DBclose(SqlConnection conn)
        {
            conn.Close();
            conn.Dispose();
        }
        
        public void DBlogcreation(SqlConnection conn, Activity act)
        {
            if (conn == null)
            {
                Console.Write("Error in connecting to the Database");
            }               
            else
            {
                SqlCommand insertCommand =
                    new SqlCommand("INSERT INTO [dbo].[logs] (id, recipientid, text, time, httpresponse) VALUES (@0, @1, @2, @3,@4)", conn);
                insertCommand.Parameters.Add(new SqlParameter("0", act.Id));
                insertCommand.Parameters.Add(new SqlParameter("1", act.Recipient.Id));
                insertCommand.Parameters.Add(new SqlParameter("2", act.Text));
                insertCommand.Parameters.Add(new SqlParameter("3", act.Timestamp));
                insertCommand.Parameters.Add(new SqlParameter("4", HttpStatusCode.OK));
                try
                {
                    insertCommand.ExecuteNonQuery();
                }
                catch(Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
            DBclose(conn);
        }

    }

}