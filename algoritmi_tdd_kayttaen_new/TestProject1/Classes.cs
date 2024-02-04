using System;
using System.Collections.Generic;

namespace ElectricityPricing.Models
{

    public enum ElectricityContractType
    {
        FixedPrice,
        MarketPrice,
        SamePrice
    }

    public class PriceDifference
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PriceDifferenceValue { get; set; }
        public ElectricityContractType CheaperContract { get; set; }

        public PriceDifference(DateTime startDate, DateTime endDate, decimal priceDifference, ElectricityContractType cheaperContract)
        {
            StartDate = startDate;
            EndDate = endDate;
            PriceDifferenceValue = priceDifference;
            CheaperContract = cheaperContract;
        }
    }

    public class MarketPrice
    {
        public List<PriceInfo> Prices { get; set; }
    }

    public class PriceInfo
    {
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class ElectricityPriceComparer
    {
        public List<PriceDifference> CompareElectricityPrices(decimal fixedPrice, MarketPrice marketPrices)
        {
            var priceDifferences = new List<PriceDifference>();

            foreach (var priceInfo in marketPrices.Prices)
            {
                var difference = priceInfo.Price - fixedPrice;
                var cheaperContract = difference > 0 ? ElectricityContractType.FixedPrice :
                                      difference < 0 ? ElectricityContractType.MarketPrice :
                                      ElectricityContractType.SamePrice;

                priceDifferences.Add(new PriceDifference(priceInfo.StartDate, priceInfo.EndDate, Math.Round(difference, 3), cheaperContract));
            }

            return priceDifferences;
        }
    }
}
