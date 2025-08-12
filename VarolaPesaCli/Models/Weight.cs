namespace VarolaPesaCli.Models
{
    public class Weight(decimal weightValue, decimal price, decimal total, decimal tara)
    {
        public decimal WeightValue { get; set; } = weightValue;
        public decimal Price { get; set; } = price;
        public decimal Total { get; set; } = total;
        public decimal Tara { get; set; } = tara;
    }

} 