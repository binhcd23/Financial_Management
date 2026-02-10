using Financial_Management_Server.DTOs.Finances;
using Financial_Management_Server.Interfaces.Finances;
using Financial_Management_Server.Models;

namespace Financial_Management_Server.Services.Finances
{
    public class DashboardService : IDashboardService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IGoalRepository _goalRepository;
        private readonly ICategoryRepository _categoryRepository;

        public DashboardService(IWalletRepository walletRepository, ITransactionRepository transactionRepository, IGoalRepository goalRepository, ICategoryRepository categoryRepository)
        {
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
            _goalRepository = goalRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<DashboardStatsDto> GetDashboardAsync(DashboardRequest request)
        {
            var now = DateOnly.FromDateTime(DateTime.Now);
            var start = request.startDate ?? new DateOnly(now.Year, now.Month, 1);
            var end = request.endDate ?? now;

            var wallet =  await _walletRepository.GetWalletsByUserIdAsync(request.userId);
            var transactions = await _transactionRepository.GetTransactionsAsync(request.userId);
            var catrgorirs = await _categoryRepository.GetCategoriesAsync();
            var goals = await _goalRepository.GetGoalsAsync(request.userId);
         
            // giao dịch start -> end
            var filteredTransactions = transactions
                 .Where(t => t.TransactionDate >= start && 
                             t.TransactionDate <= end)
                 .ToList();

            var allExpenseCategories = catrgorirs
                 .Where(c => c.Type == "Expense")
                 .ToList();

            // dùng cho biểu đồ từng mục chi tiêu
            var categoryStats = allExpenseCategories
                  .Select(cat => {
                      var categoryTransactions = filteredTransactions
                          .Where(t => t.CategoryId == cat.CategoryId);

                      return new CategoriesDto
                      {
                          CategoryId = cat.CategoryId,
                          CategoryName = cat.CategoryName ?? "Chưa đặt tên",
                          TotalAmount = categoryTransactions.Sum(t => t.Amount),
                          TransactionCount = categoryTransactions.Count()
                      };
                  })
                  .OrderByDescending(x => x.TotalAmount)
                  .ToList();

            var totalGoals = goals.Count();

            // dùng cho biểu đồ goal với tứng status
            var goalStats = goals
                .GroupBy(g => g.Status)
                .Select(group => {
                    var count = group.Count();
                    return new GoalDto
                    {
                        Status = group.Key ?? "Chưa xác nhận",
                        GoalsCount = count,
                        Percentage = totalGoals > 0
                            ? Math.Round((double)count / totalGoals * 100, 1)
                            : 0
                    };
                })
                .ToList();

            var activeAndCompletedGoals = goals
                .Where(g => g.Status == "Active" || g.Status == "Completed")
                .ToList();

            decimal totalTargetAmount = activeAndCompletedGoals.Sum(g => g.TargetAmount);
            decimal totalCurrentAmount = activeAndCompletedGoals.Sum(g => g.CurrentAmount ?? 0);

            double overallGoalProgress = totalTargetAmount > 0
                ? (double)Math.Round((totalCurrentAmount / totalTargetAmount) * 100, 1)
                : 0;

            // tổng thu
            var incomeValue = transactions
                 .Where(t => t.Category!.Type == "Income")
                 .Sum(t => t.Amount);

            // tổng chi tháng này
            var expensesValue = transactions
                .Where(t => t.Category!.Type == "Expense")
                .Sum(t => t.Amount);

          
            var savingValue = wallet
                .Where(w => w.WalletType == "Savings")
                .Sum(w => w.Balance) ?? 0;
            return new DashboardStatsDto
            {
                IncomeValue = incomeValue,
                ExpensesValue = expensesValue,
                IncomeTransactions = transactions.Where(t => t.Category!.Type == "Income").Count(),
                ExpensesTransactions = transactions.Where(t => t.Category!.Type == "Expense").Count(),
                ActiveGoalsCount = goals.Count(g => g.Status == "Active"),
                OverallGoalProgress = overallGoalProgress,
                SavingValue = savingValue,
                Categories = categoryStats,
                Goals = goalStats
            };
        }
     
    }
}
