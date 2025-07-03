namespace Projet_7.Core.Domain
{
    public class Bid
    {
        public int Id { get; set; }
        public string Account { get; set; }
        public string BidType { get; set; }
        public double? BidQuantity { get; set; }
        public double? AskQuantity { get; set; }
        public double? BidValue { get; set; }
        public double? Ask { get; set; }
        public string? Benchmark { get; set; }
        public DateTime? BidDate { get; set; }
        public string? Commentary { get; set; }
        public string? BidSecurity { get; set; }
        public string? BidStatus { get; set; }
        public string? Trader { get; set; }
        public string? Book { get; set; }
        public string? CreationName { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? RevisionName { get; set; }
        public DateTime? RevisionDate { get; set; }
        public string? DealName { get; set; }
        public string? DealType { get; set; }
        public int? SourceId { get; set; }
        public string? Side { get; set; }
    }
}