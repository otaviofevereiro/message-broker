using System;

namespace Application.Messages
{
    public class Message
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }

        public string GetFileName()
        {
            return $"{From}_{To}";
        }
    }
}
