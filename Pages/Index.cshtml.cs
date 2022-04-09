using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RestAPI.Models;
using RestAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public JsonFileService jsonService;

        public List<User> Users { get; set; }
        public List<Monkey> Monkeys { get; set; }
        public string[] Cats { get; set; }

        public IndexModel(ILogger<IndexModel> logger, JsonFileService json)
        {
            _logger = logger;
            jsonService = json;
        }

        public void OnGet()
        {
            Users = jsonService.LoadJsonFile<User>("UserList.json");
            Monkeys = jsonService.LoadJsonFile<Monkey>("monkeys.json");
            Cats = System.IO.File.ReadAllLines("wwwroot/data/cats.txt");
        }
    }
}