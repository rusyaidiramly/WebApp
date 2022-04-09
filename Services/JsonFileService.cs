using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace RestAPI.Services
{
    public class JsonFileService
    {
        private static IWebHostEnvironment HostEnv {get;set;}

        public JsonFileService(IWebHostEnvironment env)
        {
            HostEnv = env;
        }
        public static void SaveJsonFile<T>(List<T> Objects, string fileName) where T : new()
        {
            try
            {
                string jsonObj = JsonConvert.SerializeObject(Objects, Formatting.Indented);
                File.WriteAllText(Path.Combine(HostEnv.WebRootPath, "data", fileName), jsonObj);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public static List<T> LoadJsonFile<T>(string fileName) where T : new()
        {
            try
            {
                StreamReader sr = new StreamReader(Path.Combine(HostEnv.WebRootPath, "data", fileName));
                string jsonString = sr.ReadToEnd();
                sr.Close();

                return JsonConvert.DeserializeObject<List<T>>((string.IsNullOrEmpty(jsonString)) ? "[]" : jsonString);

            }
            catch (FileNotFoundException)
            {
                return JsonConvert.DeserializeObject<List<T>>("[]");
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
