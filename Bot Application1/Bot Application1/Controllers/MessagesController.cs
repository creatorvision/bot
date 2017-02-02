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
using DBLogger;
using System.Data.SqlClient;
using Bot_Application1.Controllers;

namespace Bot_Application1
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {

            if (activity.Type == ActivityTypes.Message)
            {
                DBManagement dbm = new DBManagement();
                SqlConnection conn = dbm.DBconnect();
                dbm.DBlogcreation(conn, activity);

                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                connector.Conversations.ReplyToActivity(activity.CreateReply("Yo, I heard you ", "en"));
                var reply = HandleSystemMessage(activity);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }
            else if(message.Type == ActivityTypes.Message)
            {
                Activity reply;
                if (message.Text.Contains("http") || message.Text.Contains("https") )
                {
                    string apiresponse="";
                    apiConsumer apires = new apiConsumer();
                    try
                    {
                        apires.MyWebRequest(message.Text);
                        apiresponse = apires.GetResponse();
                       if(apiresponse == null)
                        {
                            Console.WriteLine("Hitting the giving link is returning nothing");
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    
                    reply = message.CreateReply(apiresponse);
                }
                else
                {
                    reply = message.CreateReply($"You said {message.Text}");
                }

                DBManagement dbm = new DBManagement();
                SqlConnection conn = dbm.DBconnect();
                reply.Id = "reply_"+message.Id;
                dbm.DBlogcreation(conn, reply);

                return reply;

            }
            return null;
        }

    }
}