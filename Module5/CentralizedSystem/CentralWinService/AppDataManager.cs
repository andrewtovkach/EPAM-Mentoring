using System;
using System.IO;
using Newtonsoft.Json;

namespace CentralWinService
{
    public static class AppDataManager
    {
        public static void WrireToFile(string fileName, object appData)
        {
            string json = JsonConvert.SerializeObject(appData);
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            File.WriteAllText(filePath, json);
        }

        public static T ReadFromFile<T>(string fileName)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
