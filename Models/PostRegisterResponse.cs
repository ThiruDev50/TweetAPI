using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetAPI.Models
{
    public class PostRegisterResponse
    {
        [BsonId]
        public ObjectId Id { get; set; }
        
        public string UserName { get; set; }
        public int UserId { get; set; }

        public string MailId { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public long ContactNumber { get; set; }

        public string token { get; set; }
         public string ProfilePictureBase64 { get; set; }
        public string UserBio { get; set; }

    }
}
