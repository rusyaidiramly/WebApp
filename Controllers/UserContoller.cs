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
        public async Task<IActionResult> Get()
        {
            List<User> users = new List<User>();
            users = await _databaseService.FetchAll();
            return (users.Any())
            ? Ok(users)
            : NotFound(new { message = "No User" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            List<User> selectedUser = new List<User>();
            selectedUser = await _databaseService.FetchOne(id);

            return (selectedUser.Any())
            ? Ok(selectedUser[0])
            : NotFound(new { message = "Not Found" });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Get(string q)
        {
            List<User> users = new List<User>();
            users = await _databaseService.FetchMatch(q);
            return (users.Any())
            ? Ok(users)
            : NotFound(new { message = "No User Match" });
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] User value)
        {
            int status = _databaseService.EditRecord(id, value);
            return (status == 1)
            ? Ok(new { message = "Edit User Success" })
            : StatusCode(400, new { message = "Edit User Failed" });
        }


        [HttpPost]
        public IActionResult Post([FromBody] User value)
        {
            int status = _databaseService.AddRecord(value);
            return (status == 1)
            ? StatusCode(201, new { message = "User Added" })
            : StatusCode(400, new { message = "Add User Failed" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            int status = _databaseService.DeleteRecord(id);
            return (status == 1)
            ? Ok(new { message = "Delete User Success" })
            : StatusCode(400, new { message = "Delete User Failed" });
        }
    }
}
