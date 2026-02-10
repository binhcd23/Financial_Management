namespace Financial_Management_Server.DTOs.Finances
{
    public class DashboardStatsDto
    {
        public decimal IncomeValue { get; set; }
        public decimal ExpensesValue { get; set; }
        public int IncomeTransactions { get; set; }
        public int ExpensesTransactions { get; set; }
        public int ActiveGoalsCount { get; set; }
        public double OverallGoalProgress { get; set; }
        public decimal SavingValue { get; set; }
        public List<CategoriesDto> Categories { get; set; } = new();
        public List<GoalDto> Goals { get; set; } = new();
    }
    public class DashboardRequest
    {
        public int userId { get; set; }
        public DateOnly? startDate { get; set; }
        public DateOnly? endDate { get; set; }
    }
}
