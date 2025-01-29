using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherAppProject
{
    internal class FetchNotamData
    {
        private const string NotamsApiKey = "ca0702e0-5565-4c86-9b46-f6274766d1a2";

        public static async Task FetchNotams(string icaoCode)
        {
            string url = $"https://applications.icao.int/dataservices/api/notams-realtime-list?api_key={NotamsApiKey}&format=json&locations={icaoCode}";

            using HttpClient client = new();
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    try
                    {
                        // Deserialize JSON into a JsonDocument for easy processing
                        var notamsDocument = JsonDocument.Parse(responseBody);

                        // Display the NOTAMs to the console
                        Console.WriteLine($"NOTAMs for {icaoCode}:");
                        FormatNotams.DisplayNotams(notamsDocument, icaoCode);

                        // Add a debug line to confirm the save prompt execution
                        Console.WriteLine("NOTAMs displayed. Prompting for save...");

                        // Prompt to save NOTAMs data to file
                        await PromptToSaveNotams(responseBody);  // Calling method to save data
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"Error parsing JSON: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to fetch NOTAMs. Status Code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request Error: {ex.Message}");
            }
        }


        static async Task PromptToSaveNotams(string responseBody)
        {
            // Prompt the user to save the NOTAM data
            Console.WriteLine("\nWould you like to save this NOTAM data to a file? (y/n)");
            string saveResponse = Console.ReadLine()?.ToLower();

            if (saveResponse == "y")
            {
                Console.WriteLine("\nEnter the filename (without extension): ");
                string fileName = Console.ReadLine()?.Trim();

                // Use a default name if the filename is empty or invalid
                if (string.IsNullOrEmpty(fileName))
                {
                    Console.WriteLine("Invalid filename. Using default name 'NOTAMsData'.");
                    fileName = "NOTAMsData";
                }

                // Save the data to a file on the desktop
                await SaveData.SaveDataToFile(fileName, responseBody);
                Console.WriteLine("Data has been saved to your desktop.");
            }
        }
    }
}
