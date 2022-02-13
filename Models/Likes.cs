using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetAPI.Models
{
    public class Likes
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public List<string> TweetLikersUserId { get; set; }

    }
}
