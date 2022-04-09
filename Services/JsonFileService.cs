using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace RestAPI.Services
{
    public static class JsonFileService
    {

        public static void SaveJsonFile<T>(List<T> Objects, string fileName) where T : new()
        {
            try
            {
                string jsonObj = JsonConvert.SerializeObject(Objects, Formatting.Indented);
                File.WriteAllText(fileName, jsonObj);
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
                StreamReader sr = new StreamReader(fileName);
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
