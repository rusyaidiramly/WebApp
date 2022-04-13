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
            _table = _dbSession._schema.GetTable("user");
            // Users = _jsonService.LoadJsonFile<User>(fileName);
        }

        private void populate(Task<RowResult> t, List<User> user)
        {
            while (t.Result.Next())
            {
                user.Add(new User()
                {
                    UserID = Convert.ToInt32(t.Result.Current["UserID"]),
                    Name = (string)t.Result.Current["Name"],
                    Email = (string)t.Result.Current["Email"],
                    Password = (string)t.Result.Current["password"],
                    NRIC = (string)t.Result.Current["NRIC"],
                    DOB = ((DateTime)t.Result.Current["DOB"]).ToString("dd/MM/yyyy"),
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var Users = new List<User>();
            var query = _table.Select("UserID", "Email", "Password", "Name", "NRIC", "DOB").Limit(100);
            await query.ExecuteAsync().ContinueWith(t => populate(t, Users));

            return (Users.Any()) ? Ok(Users) : Ok(new { message = "No user" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var OneUser = new List<User>();
            var query = _table.Select("UserID", "Email", "Password", "Name", "NRIC", "DOB").Where("UserID=:id").Bind("id", id).Limit(1);
            await query.ExecuteAsync().ContinueWith(t => populate(t, OneUser));

            return (OneUser.Any()) ? Ok(OneUser[0]) : Ok(new { message = "Not found" });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Get(string q)
        {
            var Users = new List<User>();
            var query = _table.Select("UserID", "Email", "Password", "Name", "NRIC", "DOB").Where("Name LIKE :q OR Email LIKE :q").Bind("q", $"%{q}%");
            await query.ExecuteAsync().ContinueWith(t => populate(t, Users));

            return (Users.Any()) ? Ok(Users) : Ok(new { message = "No user" });
        }

        [HttpPost]
        public void Post([FromBody] User value)
        {
            Console.WriteLine("Inside Post Function");
            if (value.Email == null || value.Password == null || value.Name == null) return;
            User AddedUser = new User()
            {
                Email = value.Email,
                Password = value.Password,
                Name = value.Name,
                NRIC = value.NRIC,
            };
            if (value.NRIC == null && value.DOB != null) AddedUser.DOB = value.DOB;

            var query = _table.Insert("Email", "Password", "Name", "NRIC", "DOB")
                            .Values(
                                // AddedUser.Email,
                                // AddedUser.Password,
                                // AddedUser.Name,
                                // AddedUser.NRIC,
                                // WebApp.Models.User.ConvertDOBFormat(AddedUser.DOB, "yyyy-MM-dd")
                                "email@org.com", "p#ssrwods", "Nasmid", "970808104432", "1997-07-07"
                            );

            Console.WriteLine(query.ToString());

            query.Execute();

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
        public async Task<IActionResult> get(int id)
        {
            var query = _table.Delete().Where("id=:id").Bind("id", id).Limit(1);
            var result = await query.ExecuteAsync();

            return (result.AffectedItemsCount == 1) ? Ok(new { message = "Delete success" }) : Ok(new { message = "Delete failed" });
        }
    }
}
