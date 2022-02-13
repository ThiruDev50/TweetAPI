using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace TweetAPI.Models
{
    public class Tweet
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int TweetId {get;set;}
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string ProfilePictureBase64 { get; set; }

        public string TweetContent { get; set; }
        public string CommentorUserName { get; set; }
        public string CommentorUserId { get; set; }
        public string CommentorContent { get; set; }
        public string CommentorProfilePictureBase64 { get; set; }
        public string TweetCreatedTime { get; set; }
        public string TweetCreatedDMY { get; set; }
        public List<string> TweetLikersUserId { get; set; }

    }
}
