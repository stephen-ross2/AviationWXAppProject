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
        public static async Task FetchWeather(string airportCodes)
        {
            string baseUrl = "https://api.checkwx.com";
            string dataToSave = ""; // Initialize an empty string to store the data


            foreach (string code in airportCodes.Split(',')) //Loop to go over each individual airport code if multiple codes are provided
            {
                foreach (string type in new[] { "metar", "taf" }) //Loop to fetch METAR and TAF data for each airport code
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
                                var jsonDocument = JsonDocument.Parse(responseBody); // Parse the JSON response into a JsonDocument
                                var airportData = jsonDocument.RootElement.GetProperty("data")[0]; // Get the first element of the "data" array (the airport data)
                                var station = airportData.GetProperty("station"); // Get the "station" property of the airport data to display in the console
                                var airportName = station.GetProperty("name").GetString(); // Get the airport name to display in the console
                                var location = station.GetProperty("location").GetString(); // Get the airport location to display in the console


                                if (dataToSave == "")  //Ensures airport name and location are only displayed once and not in between the METAR and TAF data                                                 
                                    {
                                        Console.ForegroundColor = ConsoleColor.Cyan;
                                    Console.WriteLine($"\n\nAirport: {airportName} - Location: {location}");
                                    Console.ResetColor();
                                    dataToSave += $"\nAirport: {airportName} - Location: {location}\n\n"; 
                                }

                                
                                string weatherDetails = FormatMetar.ExtractWeatherDetails(jsonDocument, type); //Calls the ExtractWeatherDetails method from the FormatMetar class to extract and format the weather details
                                Console.WriteLine(weatherDetails);
                                dataToSave += weatherDetails; 

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
                    fileName = "WeatherData";  
                }

                // Call SaveDataToFile with the user-provided filename
                await SaveData.SaveDataToFile(fileName, dataToSave);
                Console.WriteLine("Data has been saved to your desktop.");
            }
        }
    }
}
