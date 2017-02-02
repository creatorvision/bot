using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using StockBot.Controllers;

namespace StockBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
 
        public async Task<HttpResponseMessage> Post([FromBody]Activity message)  
        {  
            if (message.Type == ActivityTypes.Message)  
            {  
                string StockRateString;  
                StockLUIS StLUIS = await GetEntityFromLUIS(message.Text);
                if (StLUIS.topScoringIntent != null)

                {
                    if (StLUIS.entities.Length != 0)
                    {
                        switch (StLUIS.topScoringIntent.intent)
                        {
                            case "StockPrice":
                                StockRateString = await GetStock(StLUIS.entities[0].entity);
                                break;
                            case "StockPrice2":
                                StockRateString = await GetStock(StLUIS.entities[0].entity);
                                break;
                            default:
                                StockRateString = "Sorry, I am not getting you...";
                                break;
                        }
                    }
                    else
                    {
                        StockRateString = "Sorry, I am not getting you...";
                    }
                }  
                else  
                {  
                    StockRateString = "Sorry, I am not getting you...";  
                }
                ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
                var reply =  HandleSystemMessage(message);
                reply.Text = StockRateString;
                connector.Conversations.ReplyToActivity(message.CreateReply(reply.Text));
                var response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }  
            else  
            {
                HandleSystemMessage(message);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }  
        }



        private async Task<string> GetStock(string StockSymbol)
        {
            double? dblStockValue = await YahooFinBot.GetStockRateAsync(StockSymbol);
            if (dblStockValue == null)
            {
                return string.Format("This \"{0}\" is not an valid stock symbol", StockSymbol);
            }
            else
            {
                return string.Format("Stock Price of {0} is {1}", StockSymbol, dblStockValue);
            }
        }

        private static async Task<StockLUIS> GetEntityFromLUIS(string Query)
        {
            Query = Uri.EscapeDataString(Query);
            StockLUIS Data = new StockLUIS();
            using (HttpClient client = new HttpClient())
            {
                string RequestURI = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/ffae71d4-c9b1-4d82-8114-08be20348e13?subscription-key=d1ef1c6cdbd34acf8f483c39a875ad4c&q=" + Query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<StockLUIS>(JsonDataResponse);
                }
            }
            return Data;
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
                // Handle knowing tha the user is pinging
            }
            else if(message.Type == ActivityTypes.Message)
            {
                return message;
            }
            return null;
        }

    }
}