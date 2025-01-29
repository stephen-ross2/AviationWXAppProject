using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherAppProject
{
    internal class FetchWeatherData
    {
        private const string ApiKey = "40cdff8569f044f79520c074dc"; //API key for CheckWX.com
        public static async Task FetchWeather (string airportCodes)
        {
            string baseUrl = "https://api.checkwx.com";
            string dataToSave = ""; // This will accumulate the data that we'll potentially save to file

            // Split the input for multiple codes
            foreach (string code in airportCodes.Split(','))
            {
                foreach (string type in new[] { "metar", "taf" })
                {
                    string endpoint = $"{baseUrl}/{type}/{code}/decoded";

                    using HttpClient client = new();
                    client.DefaultRequestHeaders.Add("X-API-Key", ApiKey);

                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(endpoint);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();

                            try
                            {
                                var jsonDocument = JsonDocument.Parse(responseBody);
                                var airportData = jsonDocument.RootElement.GetProperty("data")[0];
                                var station = airportData.GetProperty("station");
                                var airportName = station.GetProperty("name").GetString();
                                var location = station.GetProperty("location").GetString();

                                // Display the airport details once, before showing METAR/TAF data
                                if (dataToSave == "")  // Only show once for the first report
                                {
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine($"\n\nAirport: {airportName} - Location: {location}");
                                    Console.ResetColor();
                                    dataToSave += $"\nAirport: {airportName} - Location: {location}\n\n"; // Save airport info
                                }

                                // Display and accumulate METAR/TAF details
                                string weatherDetails = FormatMetar.ExtractWeatherDetails(jsonDocument, type);
                                Console.WriteLine(weatherDetails);
                                dataToSave += weatherDetails; // Accumulate to be saved to file

                                if (type == "taf")
                                {
                                    Console.WriteLine("___________________________________________________________________________________________________________________________________________________________________________");
                                    dataToSave += "\n___________________________________________________________________________________________________________________________________________________________________________\n";
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error decoding {type.ToUpper()} JSON: {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Failed to fetch {type.ToUpper()}. Status Code: {response.StatusCode}");
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"HTTP Request Error: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error fetching {type.ToUpper()}: {ex.Message}");
                    }
                }
            }
                       

            // Prompt for saving the data
            Console.WriteLine("\nWould you like to save this weather data to a file? (y/n)");
            string saveResponse = Console.ReadLine()?.ToLower();

            if (saveResponse == "y")
            {
                Console.WriteLine("\nEnter the filename (without extension): ");
                string fileName = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(fileName))
                {
                    Console.WriteLine("Invalid filename. Using default name 'WeatherData'.");
                    fileName = "WeatherData";  // Set a default filename if the input is invalid.
                }

                // Call SaveDataToFile with the user-provided filename
                await SaveData.SaveDataToFile(fileName, dataToSave);
                Console.WriteLine("Data has been saved to your desktop.");
            }
        }

       
    }

}

