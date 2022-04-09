using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using RestAPI.Models;
using RestAPI.Services;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public static readonly string fileName = "UserList.json";
        private static List<User> Users = JsonFileService.LoadJsonFile<User>(fileName);


        [HttpGet]
        public IEnumerable<User> Get()
        {
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

            JsonFileService.SaveJsonFile(Users, fileName);
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

            JsonFileService.SaveJsonFile(Users, fileName);

        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            User selectedUser = Users.Find(user => user.UserID == id);
            if (selectedUser == null) return;
            Users.Remove(selectedUser);

            JsonFileService.SaveJsonFile(Users, fileName);
        }
    }
}
