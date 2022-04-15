using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using WebApp.Models;
using WebApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Pages
{
    public class EntryModel : PageModel
    {
        // private DatabaseService dbService { get; set; }
        private JsonFileService jsonService;
        private HttpRequestService _http;
        public User usr { get; set; }
        public User updtUser { get; set; }
        public List<User> Users { get; set; }
        public List<Monkey> Monkeys { get; set; }
        public EntryModel(DatabaseService service, JsonFileService json, HttpRequestService http)
        {
            // dbService = service;
            jsonService = json;
            _http = http;
        }

        public void OnGet([FromQuery(Name = "query")] string q)
        {
            string url = "http://192.168.43.128:5001/api/user";
            if (q != null) url += "/search?q=" + q;
            Users = _http.Get<List<User>>(url).Result;
        }

        //Put & Delete handled by javascript
    }
}