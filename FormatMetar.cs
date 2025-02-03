using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherAppProject
{
   internal class FormatMetar
    {
        public static string ExtractWeatherDetails(JsonDocument jsonDocument, string type) //Method to extract and format the weather details
        {
            var data = jsonDocument.RootElement.GetProperty("data");
            string details = ""; 

            foreach (var report in data.EnumerateArray())
            {
                details += $"\n\n========= {type.ToUpper()} =========";
                details += $"\nRaw: {report.GetProperty("raw_text").GetString()}";

                if (type == "metar")
                {
                    details += DecodeMetar(report);
                }
                else if (type == "taf")
                {
                    
                    details += FormatTaf.DecodeTaf(report); 
                }
            }

            return details;
        }
        public static string DecodeMetar(JsonElement report) //Method to decode the rawd data in the JSON to a more user friendly format
        {
            string details = "\n\nDecoded METAR Details:";

            if (report.TryGetProperty("observed", out var observedTime))
            {
                DateTime observationDateTime = DateTime.Parse(observedTime.GetString());
                string formattedDate = observationDateTime.ToString("MMM dd yyyy HH:mm 'UTC'");
                details += $"\n\n  Observation Time: {formattedDate}";
            }

            if (report.TryGetProperty("temperature", out var temp))
            {
                double tempCelsius = temp.GetProperty("celsius").GetDouble();
                double tempFahrenheit = tempCelsius * 9 / 5 + 32;
                details += $"\n  Temperature: {tempCelsius}°C ({tempFahrenheit:F1}°F)";
            }

            if (report.TryGetProperty("dewpoint", out var dewpoint))
            {
                double dewCelsius = dewpoint.GetProperty("celsius").GetDouble();
                double dewFahrenheit = dewCelsius * 9 / 5 + 32;
                details += $"\n  Dewpoint: {dewCelsius}°C ({dewFahrenheit:F1}°F)";
            }

            if (report.TryGetProperty("wind", out var wind))
            {
                details += $"\n  Wind: {wind.GetProperty("degrees").GetInt32():D3}° @ {wind.GetProperty("speed_kts").GetInt32()} knots";
            }

            if (report.TryGetProperty("visibility", out var visibility))
            {
                details += $"\n  Visibility: {visibility.GetProperty("miles_float").GetDouble()} miles";
            }

            if (report.TryGetProperty("barometer", out var barometer) && barometer.ValueKind == JsonValueKind.Object)
            {
                if (barometer.TryGetProperty("hg", out var barometerHg))
                {
                    double barometerInHg = barometerHg.GetDouble();
                    details += $"\n  Altimeter: {barometerInHg:F2} inHg";
                }
                else
                {
                    details += "\n  Altimeter: 'hg' property not found.";
                }
            }
            else
            {
                details += "\n  Altimeter: Data not available.";
            }

            if (report.TryGetProperty("clouds", out var clouds) && clouds.GetArrayLength() > 0)
            {
                foreach (var cloud in clouds.EnumerateArray())
                {
                    var coverage = cloud.GetProperty("text").GetString();
                    if (cloud.TryGetProperty("base_feet_agl", out var baseFeet))
                    {
                        details += $"\n  Ceiling: {coverage} at {baseFeet.GetInt32()}ft AGL";
                    }
                    else
                    {
                        details += $"\n  Ceiling: {coverage} at cloud base height not available.";
                    }
                }
            }
            else
            {
                details += "\n  Ceiling: No data available.";
            }

            return details;
        }


    }
}
