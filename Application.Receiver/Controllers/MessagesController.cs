using Application.MessageBroker;
using Application.Messages;
using Microsoft.AspNetCore.Mvc;

namespace Application.Receiver.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageQueue messageQueue;

        public MessagesController(IMessageQueue messageQueue)
        {
            this.messageQueue = messageQueue;
        }

        [HttpPost]
        public IActionResult Post(Message message)
        {
            messageQueue.Enqueue(Queues.Messages, message);

            return Ok();
        }
    }
}
