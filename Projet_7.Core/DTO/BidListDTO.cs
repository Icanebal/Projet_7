namespace Projet_7.Core.DTO
{
    public class BidListDto
    {
        public int Id { get; set; }
        public string Account { get; set; }
        public string BidType { get; set; }
        public double? BidQuantity { get; set; }
        public double? AskQuantity { get; set; }
        public double? Bid { get; set; }
        public double? Ask { get; set; }
        public DateTime? BidListDate { get; set; }
        public string BidStatus { get; set; }
        public string Trader { get; set; }
    }
}
