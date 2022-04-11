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
        private JsonFileService _jsonService;
        private MySqlDatabase _mySqlDatabase { get; set; }

        public UserController(JsonFileService jsonService, MySqlDatabase mySqlDatabase)
        {
            _jsonService = jsonService;
            // Users = _jsonService.LoadJsonFile<User>(fileName);
            _mySqlDatabase = mySqlDatabase;
        }


        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            var Users = new List<User>();
            var cmd = _mySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM users";

            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    Users.Add(new User()
                    {
                        UserID = Convert.ToInt32(reader.GetFieldValue<UInt64>(0)),
                        Email = reader.GetFieldValue<string>(1),
                        Password = reader.GetFieldValue<string>(2),
                        Name = reader.GetFieldValue<string>(3),
                    });
                }

            return Users;
        }

        [HttpGet("{id}")]
        public async Task<User> Get(int id)
        {
            User SelectedUser = null;
            var cmd = _mySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM users WHERE id=" + id.ToString() + " LIMIT 1";

            using (var reader = await cmd.ExecuteReaderAsync())
                if (await reader.ReadAsync())
                {
                    SelectedUser = new User()
                    {
                        UserID = Convert.ToInt32(reader.GetFieldValue<UInt64>(0)),
                        Email = reader.GetFieldValue<string>(1),
                        Password = reader.GetFieldValue<string>(2),
                        Name = reader.GetFieldValue<string>(3),
                    };
                }

            return SelectedUser;
        }
        /*
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
                }*/
    }
}
