using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace RestAPI.Services
{
    public class JsonFileService
    {
        private IWebHostEnvironment HostEnv { get; }

        public JsonFileService(IWebHostEnvironment webHostEnvironment)
        {
            HostEnv = webHostEnvironment;
        }
        public void SaveJsonFile<T>(List<T> Objects, string fileName) where T : new()
        {
            try
            {
                string jsonObj = JsonConvert.SerializeObject(Objects, Formatting.Indented);
                File.WriteAllText(Path.Combine(HostEnv.WebRootPath, "data", fileName), jsonObj);
                // File.WriteAllText($"data/{fileName}", jsonObj);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public List<T> LoadJsonFile<T>(string fileName) where T : new()
        {
            try
            {
                StreamReader sr = new StreamReader(Path.Combine(HostEnv.WebRootPath, "data", fileName));
                // StreamReader sr = new StreamReader($"data/{fileName}");
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
