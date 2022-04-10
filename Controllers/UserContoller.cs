using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using WebApp.Models;
using WebApp.Services;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public static readonly string fileName = "UserList.json";
        public JsonFileService _jsonService;
        private MySqlDatabase _mySqlDatabase { get; set; }
        private List<User> Users;

        public UserController(JsonFileService jsonService, MySqlDatabase mySqlDatabase)
        {
            _jsonService = jsonService;
            // Users = _jsonService.LoadJsonFile<User>(fileName);
            _mySqlDatabase = mySqlDatabase;
        }


        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            Users = new List<User>();
            var cmd = _mySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM users";

            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    User t = new()
                    {
                        UserID = Convert.ToInt32(reader.GetFieldValue<UInt64>(0)),
                        Email = reader.GetFieldValue<string>(1)
                    };

                    Users.Add(t);
                }
            return Users;
        }

        [HttpGet("{id}")]
        public User Get(int id)
        {
            return Users.Find(user => user.UserID == id);
        }

        [HttpPost]
        public void Post([FromBody] User value)
        {
            int currentID = (Users?.Any() == true) ? Users.Last().UserID : 0;
            Users.Add(new User
            {
                UserID = currentID + 1,
                Name = value.Name,
                Email = value.Email,
                Password = value.Password,
                NRIC = value.NRIC,
                DOB = value.DOB
            });

            _jsonService.SaveJsonFile(Users, fileName);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] User value)
        {
            User selectedUser = Users.Find(user => user.UserID == id);
            if (selectedUser == null) return;
            if (value.Name != null) selectedUser.Name = value.Name;
            if (value.Email != null) selectedUser.Email = value.Email;
            if (value.Password != null) selectedUser.Password = value.Password;
            if (value.NRIC != null) selectedUser.NRIC = value.NRIC;
            if (value.NRIC == null && value.DOB != null) selectedUser.DOB = value.DOB;

            _jsonService.SaveJsonFile(Users, fileName);

        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            User selectedUser = Users.Find(user => user.UserID == id);
            if (selectedUser == null) return;
            Users.Remove(selectedUser);

            _jsonService.SaveJsonFile(Users, fileName);
        }
    }
}
