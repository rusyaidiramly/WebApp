using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using WebApp.Models;
using WebApp.Services;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Relational;
using MySqlX.XDevAPI.Common;
using MySqlX.XDevAPI.CRUD;
using System.Diagnostics;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserXController : ControllerBase
    {
        public static readonly string fileName = "UserList.json";
        private JsonFileService _jsonService;
        private MySqlDatabase _mySqlDatabase { get; set; } //old way
        private dbSession _dbSession { get; set; } //new way
        private Table _table { get; set; }
        public UserXController(JsonFileService jS, MySqlDatabase mysqlS, dbSession dbS)
        {
            _jsonService = jS;
            _mySqlDatabase = mysqlS;
            _dbSession = dbS;
            _table = _dbSession._schema.GetTable("Users");
            // Users = _jsonService.LoadJsonFile<User>(fileName);
        }


        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            var Users = new List<User>();
            var query = _table.Select("id,email,password,name,nric,dob");

            await query.ExecuteAsync().ContinueWith(t =>
            {
                while (t.Result.Next())
                {
                    Users.Add(new User()
                    {
                        UserID = Convert.ToInt32(t.Result.Current["id"]),
                        Name = (string)t.Result.Current["name"],
                        Email = (string)t.Result.Current["email"],
                        Password = (string)t.Result.Current["password"],
                        NRIC = (string)t.Result.Current["nric"],
                        DOB = (DateTime)t.Result.Current["dob"],
                    });
                }
            });

            return Users;
        }

        [HttpGet("{id}")]
        public async Task<User> Get(int id)
        {
            User OneUser = null;
            var query = _table.Select("id,email,password,name,nric,dob").Where("id=:id").Bind("id", id).Limit(1);
            await query.ExecuteAsync().ContinueWith(t =>
            {
                if (t.Result.Next())
                {
                    OneUser = new User()
                    {
                        UserID = Convert.ToInt32(t.Result.Current["id"]),
                        Name = (string)t.Result.Current["name"],
                        Email = (string)t.Result.Current["email"],
                        Password = (string)t.Result.Current["password"],
                        NRIC = (string)t.Result.Current["nric"],
                        DOB = (DateTime)t.Result.Current["dob"],
                    };
                }
            });

            return OneUser;

        }

        [HttpGet("search")]
        public async Task<IEnumerable<User>> Get(string q)
        {
            var Users = new List<User>();
            var query = _table.Select("id,email,password,name,nric,dob").Where("name LIKE :q OR email like :q").Bind("q", $"%{q}%");

            await query.ExecuteAsync().ContinueWith(t =>
            {
                while (t.Result.Next())
                {
                    Users.Add(new User()
                    {
                        UserID = Convert.ToInt32(t.Result.Current["id"]),
                        Name = (string)t.Result.Current["name"],
                        Email = (string)t.Result.Current["email"],
                        Password = (string)t.Result.Current["password"],
                        NRIC = (string)t.Result.Current["nric"],
                        DOB = (DateTime)t.Result.Current["dob"],
                    });
                }
            });

            return Users;
        }

        [HttpPost]
        public async void Post([FromBody] User value)
        {
            if (value.Email == null || value.Password == null || value.Name == null) return;
            User AddedUser = new User()
            {
                Email = value.Email,
                Password = value.Password,
                Name = value.Name,
                NRIC = value.NRIC,
            };
            if (value.NRIC == null && value.DOB != null) AddedUser.DOB = value.DOB;
            var query = _table.Insert("email,password,name,nric,dob")
                            .Values(AddedUser.Email,AddedUser.Password,AddedUser.Name,AddedUser.NRIC,DateTime.Parse(AddedUser.DOB).ToString("yy-MM-dd"));
            
            await query.ExecuteAsync();
            
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] User value)
        {
            if (
                value.Email == null
                && value.Password == null
                && value.Name == null
                && value.NRIC == null
                && value.DOB == null
                ) return;

            User selectedUser = new User();
            List<string> fields = new List<string>();

            if (value.Email != null)
            {
                selectedUser.Email = value.Email;
                fields.Add($"email='{selectedUser.Email}'");
            }
            if (value.Password != null)
            {
                selectedUser.Password = value.Password;
                fields.Add($"password='{selectedUser.Password}'");
            }
            if (value.Name != null)
            {
                selectedUser.Name = value.Name;
                fields.Add($"name='{selectedUser.Name}'");
            }
            if (value.NRIC != null)
            {
                selectedUser.NRIC = value.NRIC;
                fields.Add($"nric='{selectedUser.NRIC}'");
            }
            if (value.DOB != null)
            {
                selectedUser.DOB = value.DOB;
                fields.Add($"dob=STR_TO_DATE('{selectedUser.DOB}', '%d/%m/%Y')");
            }

            var cmd = _mySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"UPDATE users SET " + String.Join(",", fields) + " WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var cmd = _mySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"DELETE FROM users WHERE id=@id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
