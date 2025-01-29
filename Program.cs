using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherAppProject;

class Program
{
    static async Task Main(string[] args)
    {
        bool showMenu = true;

        while (showMenu)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("======= FLIGHT FORECASTS =======");
            Console.ResetColor();
            Console.WriteLine("\nPlease select an option below by typing the number and pressing ENTER:");
            Console.WriteLine();
            Console.WriteLine("1. Check current and forecasted weather conditions at an airport (METARs / TAFs)");
            Console.WriteLine("2. Check NOTAMs");
            Console.WriteLine("3. Exit");
            Console.WriteLine();

            string userSelection = Console.ReadLine();

            switch (userSelection)
            {
                case "1":
                    Console.WriteLine("Enter the ICAO code(s) (e.g., KJFK, KLAX). Separate multiple codes with a comma:");
                    string airportCodes = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(airportCodes))
                    {
                        Console.WriteLine("Invalid input. Please enter at least one ICAO code.");
                    }
                    else
                    {
                        await FetchWeatherData.FetchWeather(airportCodes);
                    }
                    break;

                case "2":
                    Console.WriteLine("Enter the ICAO airport code (e.g., KJFK):");
                    string notamCode = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(notamCode))
                    {
                        await FetchNotamData.FetchNotams(notamCode.ToUpper());
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a valid ICAO airport code.");
                    }
                    break;

                case "3":
                    showMenu = false;
                    Console.WriteLine("Thank you for using the Aviation Planning App. Safe travels!");
                    break;

                default:
                    Console.WriteLine("Invalid selection. Please try again.");
                    break;
            }

            if (showMenu)
            {
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
            }
        }
    }
}





