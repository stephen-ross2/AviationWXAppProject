using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherAppProject
{
    internal class SaveData
    {

       public static async Task SaveDataToFile(string fileName, string data)
        {
            // Ensure the filename doesn't contain any invalid characters
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_'); // Replace invalid characters with an underscore
            }

            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName + ".txt");

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                await writer.WriteAsync(data);
            }
        }

    }
}
