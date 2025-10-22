namespace CaffePOS.Model.DTOs.Requests
{
    public class PaymentPostDto
    {
        public decimal amount { get; set; }
        public string? method { get; set; }
        public string? notes { get; set; }
    }
}
