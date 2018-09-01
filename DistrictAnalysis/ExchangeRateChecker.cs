
using DistrictAnalysis;
using System;
using System.Net;

public static class ExchangeRateChecker
{
    public const string ADDRESS = "https://www.xe.com/currencyconverter/convert/?Amount=1&From=USD&To=EUR";
    public const string TARGET_STRING = "class='uccResultUnit' data-amount=";

    public static float GetEurosPerDollar()
    {
        Console.Write("Downloading exchange rate... ");
        const float BASE_VALUE = 0.854099f;
        using (var client = new WebClient())
        {
            try
            {
                string page = client.DownloadString(ADDRESS);

                int index = page.IndexOf(TARGET_STRING);

                if(index == -1)
                {
                    Data.WriteColour("Downloaded page for exchange rate but failed to find data.", ConsoleColor.Red);
                    return BASE_VALUE;
                }
                else
                {
                    try
                    {
                        string part = page.Substring(index + TARGET_STRING.Length, 40);
                        int end = part.IndexOf('>');

                        string stringExchangeRate = part.Substring(1, end - 2);
                        float rate = float.Parse(stringExchangeRate);
                        Console.WriteLine("Done! Rate is " + rate + " euros per dollar");
                        return rate;
                    }
                    catch(Exception e)
                    {
                        Data.WriteColour("Exchange page downloaded, but parsing failed. Using default value.", ConsoleColor.Red);
                        return BASE_VALUE;
                    }                    
                }
            }
            catch (WebException e)
            {
                Data.WriteColour("Failed to download latest exchange rate, using default value...", ConsoleColor.Red);
                return BASE_VALUE;
            }
        }
    }
}
