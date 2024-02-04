using System;
using Newtonsoft.Json;
using ElectricityPricing.Models;

class Program
{
    static void Main(string[] args)
    {
        ElectricityPriceComparer electricityPriceComparer = new ElectricityPriceComparer();

        decimal fixedPrice = 17.62m;

        string jsonInput = @"{
            ""prices"": [
                { ""price"": 13.494, ""startDate"": ""2022-11-14T22:00:00.000Z"", ""endDate"": ""2022-11-14T23:00:00.000Z"" },
                { ""price"": 17.62, ""startDate"": ""2022-11-14T21:00:00.000Z"", ""endDate"": ""2022-11-14T22:00:00.000Z"" }
            ]
        }";

        MarketPrice marketPrices = JsonConvert.DeserializeObject<MarketPrice>(jsonInput);

        var priceDifferences = electricityPriceComparer.CompareElectricityPrices(fixedPrice, marketPrices);

        foreach (var difference in priceDifferences)
        {
            Console.WriteLine($"Aikavälillä {difference.StartDate} - {difference.EndDate} " +
                              $"hintaero on {difference.PriceDifferenceValue} senttiä. " +
                              $"Halvempi sopimus: {difference.CheaperContract}");
        }
    }
}