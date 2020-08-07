using MongoDB.Bson;
using System;

namespace Application.Data
{
    public class Message
    {
        public ObjectId Id { get; set; }
        public string Text { get; set; }
        public string To { get; set; }
    }
}
