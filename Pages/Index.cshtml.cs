using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestAPI.Models;
using RestAPI.Services;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public JsonFileService _jsonService;
        public List<User> Users { get; set; }


        public IndexModel(ILogger<IndexModel> logger, JsonFileService jsonService)
        {
            _logger = logger;
            _jsonService = jsonService;
        }

        public void OnGet()
        {
            Users = _jsonService.LoadJsonFile<User>("UserList.json");
        }
    }
}
