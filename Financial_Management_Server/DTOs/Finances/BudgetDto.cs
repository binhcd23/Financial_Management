using Financial_Management_Server.Models;

namespace Financial_Management_Server.DTOs.Finances
{
    public class BudgetDto
    {
        public int? BudgetId { get; set; }
        public int? UserId { get; set; }
        public int? CategoryId { get; set; }
        public string ? CategoryName { get; set; }
        public decimal AmountLimit { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public BudgetDto() { }
        public BudgetDto(Budget budget)
        {
            BudgetId = budget.BudgetId;
            UserId = budget.UserId;
            CategoryId = budget.CategoryId;
            CategoryName = budget.Category?.CategoryName;
            AmountLimit = budget.AmountLimit;
            StartDate = budget.StartDate;
            EndDate = budget.EndDate;
        }

        public Budget ToBudget()
        {
            return new Budget
            {
                UserId = UserId,
                CategoryId = CategoryId,
                AmountLimit = AmountLimit,
                StartDate = StartDate,
                EndDate = EndDate,
            };
        }
    }
    public class BudgetResponses
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
