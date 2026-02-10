namespace Financial_Management_Server.DTOs
{
    public class TransferRequest
    {
        public int sentWalletId {  get; set; }
        public int receivedWalletId { get; set; }
        public decimal amount { get; set; }
    }
    public class TransferResponses
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
