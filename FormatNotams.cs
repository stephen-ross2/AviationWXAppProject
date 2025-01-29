using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherAppProject
{
    internal class FormatNotams
    {

        public static void DisplayNotams(JsonDocument notamsDocument, string icaoCode)
        {
            var notamsArray = notamsDocument.RootElement;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nNOTAMs for {icaoCode.ToUpper()}:\n");
            Console.ResetColor();

            foreach (var notam in notamsArray.EnumerateArray())
            {
                // Extract details
                string id = notam.GetProperty("id").GetString();
                string message = notam.TryGetProperty("message", out var messageProp) ? messageProp.GetString() : "No message available";

                string startDate = notam.TryGetProperty("startdate", out var startProp) ?
                    DateTime.Parse(startProp.GetString()).ToString("MMM dd, yyyy HH:mm 'UTC'") :
                    "N/A";

                string endDate = notam.TryGetProperty("enddate", out var endProp) ?
                    DateTime.Parse(endProp.GetString()).ToString("MMM dd, yyyy HH:mm 'UTC'") :
                    "N/A";

                // Display extracted information
                Console.WriteLine($"\nNOTAM ID: {id}");
                Console.WriteLine($"Validity: {startDate} to {endDate}");
                Console.WriteLine($"Message: {message}\n");
                Console.WriteLine(new string('-', 40));
            }
        }
    }
}


      
