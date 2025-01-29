using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherAppProject
{
    internal class FormatTaf
    {
       public static string DecodeTaf(JsonElement report)
        {
            string details = "\n\nDecoded TAF Details:";

            if (report.TryGetProperty("forecast", out var forecasts))
            {
                foreach (var forecast in forecasts.EnumerateArray())
                {
                    if (forecast.TryGetProperty("timestamp", out var timestamp))
                    {
                        string fromTime = timestamp.GetProperty("from").GetString();
                        string toTime = timestamp.GetProperty("to").GetString();
                        DateTime fromDateTime = DateTime.Parse(fromTime);
                        DateTime toDateTime = DateTime.Parse(toTime);
                        details += $"\n\n  Forecast: From {fromDateTime:MMM dd yyyy HH:mm 'UTC'} to {toDateTime:MMM dd yyyy HH:mm 'UTC'}";
                    }

                    if (forecast.TryGetProperty("wind", out var wind))
                    {
                        int windDirection = wind.GetProperty("degrees").GetInt32();
                        int windSpeed = wind.GetProperty("speed_kts").GetInt32();
                        string formattedWindDirection = windDirection.ToString("D3");
                        details += $"\n\n    Wind: {formattedWindDirection} @ {windSpeed} knots";
                    }

                    if (forecast.TryGetProperty("visibility", out var visibility))
                    {
                        details += $"\n    Visibility: {visibility.GetProperty("miles").GetString()} miles";
                    }

                    if (forecast.TryGetProperty("clouds", out var clouds) && clouds.GetArrayLength() > 0)
                    {
                        foreach (var cloud in clouds.EnumerateArray())
                        {
                            var coverage = cloud.GetProperty("text").GetString();
                            if (cloud.TryGetProperty("base_feet_agl", out var baseFeet))
                            {
                                details += $"\n    Ceiling: {coverage} at {baseFeet.GetInt32()}ft AGL";
                            }
                            else
                            {
                                details += $"\n    Ceiling: {coverage} at cloud base height not available.";
                            }
                        }
                    }
                    else
                    {
                        details += "\n    Ceiling: No data available.";
                    }
                }
            }

            return details;
        }




    }
}
