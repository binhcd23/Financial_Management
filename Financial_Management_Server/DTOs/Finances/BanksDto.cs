namespace Financial_Management_Server.DTOs.Finances
{
    public class BanksDto
    {
        public string Code { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public List<BankListDto> Data { get; set; } = new();
    }

    public class BankListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Bin { get; set; } = string.Empty; 
        public string ShortName { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;  
        public int IsTransfer { get; set; }   
    }
}