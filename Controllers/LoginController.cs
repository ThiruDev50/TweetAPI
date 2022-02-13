using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TweetAPI.Models;

namespace TweetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
      
        }

       

        [HttpPost]
        
        
        public JsonResult Post(Login login)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("TweetAppCon"));


            var collection = dbClient.GetDatabase("TweetDB").GetCollection<PostRegisterResponse>("UsersInfo");
            try
            {
                var builder = Builders<PostRegisterResponse>.Filter;
                var condition = Builders<PostRegisterResponse>.Filter.And(builder.Eq("MailId", login.MailId), builder.Eq("Password", login.Password));
                var fields = Builders<PostRegisterResponse>.Projection.Include("MailId").Include("UserName").Include("Gender").Include("Password").Include("ContactNumber").Include("ProfilePictureBase64").Include("UserId").Include("UserBio");
                var results = collection.Find(condition).Project<PostRegisterResponse>(fields).ToList().AsQueryable();

                if (results.Count() == 0)
                {
                    return new JsonResult("UnAuthorized");

                }
                else
                {
                    var token = GenerateJSONWebToken();
                    return new JsonResult(results);
                }

            }
            catch(Exception ex)
            {
                return new JsonResult(ex);
            }
        }
        [HttpPost]
        [Route("GetToken")]
        public JsonResult GetToken(Login login)
        {
                var token = GenerateJSONWebToken();
                return new JsonResult(token);
        }
        [HttpPost]
        [Route("ForgotPassword")]
        public JsonResult ForgotPassword(Register login)
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("TweetAppCon"));


            var collection = dbClient.GetDatabase("TweetDB").GetCollection<Register>("UsersInfo");
            try
            {
                var builder = Builders<Register>.Filter;
                var condition = Builders<Register>.Filter.And(builder.Eq("MailId", login.MailId), builder.Eq("ContactNumber", login.ContactNumber));
                var fields = Builders<Register>.Projection.Include("MailId");
                var results = collection.Find(condition).Project<Register>(fields).ToList().AsQueryable();


                if (results.Count() == 0)
                {
                    return new JsonResult("UnAuthorized");

                }
                return new JsonResult(results);

            }
            catch (Exception ex)
            {
                return new JsonResult(ex);
            }
        }
        [HttpPost]
        [Route("UpdatePassword")]
        public JsonResult UpdatePassword(Register register)
        {
            try
            {
                MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("TweetAppCon"));
                var filter = Builders<Register>.Filter.Eq("MailId", register.MailId);
                var update = Builders<Register>.Update.Set("Password", register.Password);
                dbClient.GetDatabase("TweetDB").GetCollection<Register>("UsersInfo").UpdateOne(filter, update);
                return new JsonResult("Updated successfully");

            }
            catch (Exception ex)
            {
                return new JsonResult(ex);
            }
        }
        private string GenerateJSONWebToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("DarkSecretInTheLight"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                null,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
