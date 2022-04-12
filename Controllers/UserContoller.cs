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
            cmd.CommandText = @"SELECT * FROM users;";

            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    Users.Add(new User()
                    {
                        UserID = Convert.ToInt32(reader["id"]),
                        Email = DBHelper.ConvertFromDBVal<string>(reader["email"]),
                        Password = DBHelper.ConvertFromDBVal<string>(reader["password"]),
                        Name = DBHelper.ConvertFromDBVal<string>(reader["name"]),
                        NRIC = DBHelper.ConvertFromDBVal<string>(reader["nric"]),
                        DOB = DBHelper.ConvertFromDBVal<DateTime>(reader["dob"]),
                    });
                }

            return Users;
        }

        [HttpGet("{id}")]
        public async Task<User> Get(int id)
        {
            User SelectedUser = null;
            var cmd = _mySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM users WHERE id=@id LIMIT 1;";
            cmd.Parameters.AddWithValue("@id", id);

            using (var reader = await cmd.ExecuteReaderAsync())
                if (await reader.ReadAsync())
                {
                    SelectedUser = new User()
                    {
                        UserID = Convert.ToInt32(reader["id"]),
                        Email = DBHelper.ConvertFromDBVal<string>(reader["email"]),
                        Password = DBHelper.ConvertFromDBVal<string>(reader["password"]),
                        Name = DBHelper.ConvertFromDBVal<string>(reader["name"]),
                        NRIC = DBHelper.ConvertFromDBVal<string>(reader["nric"]),
                        DOB = DBHelper.ConvertFromDBVal<DateTime>(reader["dob"]),
                    };
                }

            return SelectedUser;
        }

        [HttpGet("search")]
        public async Task<IEnumerable<User>> Get(string q)
        {
            var Users = new List<User>();
            var cmd = _mySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT * FROM users WHERE name LIKE '%@q%' OR email LIKE '%@q%'";
            cmd.Parameters.AddWithValue("@q", q);

            using (var reader = await cmd.ExecuteReaderAsync())
                while (await reader.ReadAsync())
                {
                    Users.Add(new User()
                    {
                        UserID = DBHelper.ConvertFromDBVal<int>(reader["id"]),
                        Email = DBHelper.ConvertFromDBVal<string>(reader["email"]),
                        Password = DBHelper.ConvertFromDBVal<string>(reader["password"]),
                        Name = DBHelper.ConvertFromDBVal<string>(reader["name"]),
                        NRIC = DBHelper.ConvertFromDBVal<string>(reader["nric"]),
                        DOB = DBHelper.ConvertFromDBVal<DateTime>(reader["dob"]),
                    });
                }

            return Users;
        }

        [HttpPost]
        public void Post([FromBody] User value)
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

            var cmd = _mySqlDatabase.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"INSERT INTO users(email,password,name,nric,dob)"
                                + @"VALUES (@email,@pwd,@name,@nric,STR_TO_DATE(@dob, '%d/%m/%Y'));";

            cmd.Parameters.AddWithValue("@email", AddedUser.Email);
            cmd.Parameters.AddWithValue("@pwd", AddedUser.Password);
            cmd.Parameters.AddWithValue("@name", AddedUser.Name);
            cmd.Parameters.AddWithValue("@nric", AddedUser.NRIC);
            cmd.Parameters.AddWithValue("@dob", AddedUser.DOB);

            cmd.ExecuteNonQuery();
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
