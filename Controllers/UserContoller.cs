using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using WebApp.Models;
using WebApp.Services;
using Newtonsoft.Json;
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
        // private MySqlDatabase _mySqlDatabase { get; set; }
        private DatabaseService _databaseService { get; }

        public UserController(JsonFileService jsonService, DatabaseService dbService)
        {
            _jsonService = jsonService;
            _databaseService = dbService;
            // Users = _jsonService.LoadJsonFile<User>(fileName);
            // _mySqlDatabase = mySqlDatabase;

        }


        [HttpGet]
        public IActionResult Get()
        {
            List<User> users = new List<User>();
            users = _databaseService.FetchAll();
            return (users.Any())
            ? Ok(users)
            : Ok(new { message = "No User", body = users });
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            List<User> selectedUser = new List<User>();
            selectedUser = _databaseService.FetchOne(id);

            return (selectedUser.Any())
            ? Ok(selectedUser[0])
            : Ok(new { message = "Not Found", success = false });
        }

        [HttpGet("search")]
        public IActionResult Get(string q)
        {
            List<User> users = new List<User>();
            users = _databaseService.FetchMatch(q);
            return (users.Any())
            ? Ok(users)
            : Ok(new { message = "No User Match", body = users });
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] User value)
        {
            int status = _databaseService.EditRecord(id, value);
            return (status == 1)
            ? Ok(new { message = "Edit User Success", body = _databaseService.FetchOne(id).First(), success = true })
            : Ok(new { message = "Edit User Failed", success = false });
        }


        [HttpPost]
        public IActionResult Post([FromBody] User value)
        {
            int status = _databaseService.AddRecord(value);
            return (status == 1)
            ? Ok(new { message = "User Added", success = true })
            : Ok(new { message = "Add User Failed" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            int status = _databaseService.DeleteRecord(id);
            return (status == 1)
            ? Ok(new { message = "Delete User Success", success = true })
            : Ok(new { message = "Delete User Failed", success = false });
        }
    }
}
