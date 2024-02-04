using Xunit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

public class ElectricityPriceComparerTests
{
    // Testi 1: Tyhja
    [Fact]
    public void CompareElectricityPrices_ShouldReturnEmptyList_WhenMarketPriceListIsEmpty()
    {
        var fixedPrice = 10m;
        var emptyMarketPrices = new MarketPrice { Prices = new List<PriceInfo>() };
        var comparer = new ElectricityPriceComparer();

        var result = comparer.CompareElectricityPrices(fixedPrice, emptyMarketPrices);

        Assert.Empty(result);
    }

    // Testi 2: Kiintea hinta halvempi
    [Fact]
    public void CompareElectricityPrices_ShouldIdentifyFixedPriceAsCheaper_WhenFixedPriceIsLower()
    {
        var fixedPrice = 10m;
        var jsonInput = @"{
            ""prices"": [
                { ""price"": 13.494, ""startDate"": ""2022-11-14T22:00:00.000Z"", ""endDate"": ""2022-11-14T23:00:00.000Z"" },
                { ""price"": 17.62, ""startDate"": ""2022-11-14T21:00:00.000Z"", ""endDate"": ""2022-11-14T22:00:00.000Z"" }
            ]
        }";
        var marketPrices = JsonConvert.DeserializeObject<MarketPrice>(jsonInput);
        var comparer = new ElectricityPriceComparer();

        var result = comparer.CompareElectricityPrices(fixedPrice, marketPrices);

        Assert.All(result, item => Assert.True(item.PriceDifferenceValue > 0));
        Assert.All(result, item => Assert.Equal(ElectricityContractType.FixedPrice, item.CheaperContract));
    }

    // Testi 3: Porssihinta halvempi
    [Fact]
    public void CompareElectricityPrices_ShouldIdentifyMarketPriceAsCheaper_WhenMarketPriceIsLower()
    {
        var fixedPrice = 20m;
        var jsonInput = @"{
            ""prices"": [
                { ""price"": 13.494, ""startDate"": ""2022-11-14T22:00:00.000Z"", ""endDate"": ""2022-11-14T23:00:00.000Z"" },
                { ""price"": 17.62, ""startDate"": ""2022-11-14T21:00:00.000Z"", ""endDate"": ""2022-11-14T22:00:00.000Z"" }
            ]
        }";
        var marketPrices = JsonConvert.DeserializeObject<MarketPrice>(jsonInput);
        var comparer = new ElectricityPriceComparer();

        var result = comparer.CompareElectricityPrices(fixedPrice, marketPrices);

        Assert.All(result, item => Assert.True(item.PriceDifferenceValue < 0));
        Assert.All(result, item => Assert.Equal(ElectricityContractType.MarketPrice, item.CheaperContract));
    }

    // Testi 4: Samat hinnat
    [Fact]
    public void CompareElectricityPrices_ShouldIdentifySamePrice_WhenPricesAreEqual()
    {

        var fixedPrice = 15m;
        var jsonInput = @"{
            ""prices"": [
                { ""price"": 15, ""startDate"": ""2022-11-14T22:00:00.000Z"", ""endDate"": ""2022-11-14T23:00:00.000Z"" },
                { ""price"": 15, ""startDate"": ""2022-11-14T21:00:00.000Z"", ""endDate"": ""2022-11-14T22:00:00.000Z"" }
            ]
        }";
        var marketPrices = JsonConvert.DeserializeObject<MarketPrice>(jsonInput);
        var comparer = new ElectricityPriceComparer();

        var result = comparer.CompareElectricityPrices(fixedPrice, marketPrices);

        Assert.All(result, item => Assert.Equal(0, item.PriceDifferenceValue));
        Assert.All(result, item => Assert.Equal(ElectricityContractType.SamePrice, item.CheaperContract));
    }

    // Testi 5: Aikavalien kasittely
    [Fact]
    public void CompareElectricityPrices_ShouldHandleTimePeriodsCorrectly()
    {
        var fixedPrice = 10m;
        var jsonInput = @"{
            ""prices"": [
                { ""price"": 9, ""startDate"": ""2022-11-14T22:00:00.000Z"", ""endDate"": ""2022-11-14T23:00:00.000Z"" },
                { ""price"": 11, ""startDate"": ""2022-11-14T21:00:00.000Z"", ""endDate"": ""2022-11-14T22:00:00.000Z"" }
            ]
        }";
        var marketPrices = JsonConvert.DeserializeObject<MarketPrice>(jsonInput);
        var comparer = new ElectricityPriceComparer();

        var result = comparer.CompareElectricityPrices(fixedPrice, marketPrices);

        Assert.Equal(2, result.Count);
        Assert.Equal(ElectricityContractType.MarketPrice, result[0].CheaperContract);
        Assert.Equal(ElectricityContractType.FixedPrice, result[1].CheaperContract);
    }

    // Testi 6: Pyoristys kolmeen desimaaliin
    [Fact]
    public void CompareElectricityPrices_ShouldRoundPriceDifferencesCorrectly()
    {
        var fixedPrice = 10.1234m;
        var jsonInput = @"{
            ""prices"": [
                { ""price"": 10.1236, ""startDate"": ""2022-11-14T22:00:00.000Z"", ""endDate"": ""2022-11-14T23:00:00.000Z"" }
            ]
        }";
        var marketPrices = JsonConvert.DeserializeObject<MarketPrice>(jsonInput);
        var comparer = new ElectricityPriceComparer();

        var result = comparer.CompareElectricityPrices(fixedPrice, marketPrices);

        Assert.Single(result);
        Assert.Equal(0.000m, result[0].PriceDifferenceValue);
    }

    // Testi 7: ???? inputit
    [Fact]
    public void CompareElectricityPrices_ShouldHandleInvalidInputGracefully()
    {
        var fixedPrice = -5m;
        var jsonInput = @"{
            ""prices"": [
                { ""price"": -10, ""startDate"": ""2022-11-14T22:00:00.000Z"", ""endDate"": ""2022-11-14T23:00:00.000Z"" }
            ]
        }";
        var marketPrices = JsonConvert.DeserializeObject<MarketPrice>(jsonInput);
        var comparer = new ElectricityPriceComparer();

    }
}