using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using RestAPI.Models;
using RestAPI.Services;

namespace RestAPI.Controllers
{
    [Route("api/User/{id}/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        public static readonly string fileName = "MessageList.json";
        private static List<Message> Messages = JsonFileService.LoadJsonFile<Message>(fileName);

        [HttpGet]
        public IEnumerable<Message> Get()
        {
            return Messages.FindAll(
                    message => message.AuthorID == int.Parse(RouteData.Values["id"].ToString())
            );
        }

        [HttpGet("{mId}")]
        public Message Get(string mId)
        {
            return Messages.Find(message
                    => message.MessageID == mId
                    && message.AuthorID == int.Parse(RouteData.Values["id"].ToString())
            );
        }

        [HttpPost]
        public void Post([FromBody] Message value)
        {
            Messages.Add(new Message
            {
                MessageID = RndService.RndAlphaNum(8),
                AuthorID = int.Parse(RouteData.Values["id"].ToString()),
                PlainMessage = value.PlainMessage
            });

            JsonFileService.SaveJsonFile(Messages, fileName);
        }

        [HttpPut("{mId}")]
        public void Put(string mId, [FromBody] Message value)
        {
            Message selectedMessage = Messages.Find(message
                                        => message.MessageID == mId
                                        && message.AuthorID == int.Parse(RouteData.Values["id"].ToString())
                                    );

            if (selectedMessage == null) return;
            if (value.PlainMessage != null) selectedMessage.PlainMessage = value.PlainMessage;

            JsonFileService.SaveJsonFile(Messages, fileName);

        }

        [HttpDelete("{mId}")]
        public void Delete(string mId)
        {
            Message selectedMessage = Messages.Find(message
                                        => message.MessageID == mId
                                        && message.AuthorID == int.Parse(RouteData.Values["id"].ToString())
                                    );

            if (selectedMessage == null) return;
            Messages.Remove(selectedMessage);

            JsonFileService.SaveJsonFile(Messages, fileName);
        }
    }
}
