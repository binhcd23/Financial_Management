using Financial_Management_Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class GoalDeadlineWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<GoalDeadlineWorker> _logger;
    public GoalDeadlineWorker(IServiceProvider serviceProvider, ILogger<GoalDeadlineWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker bắt đầu chu kỳ quét lúc: {time}", DateTimeOffset.Now);

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PersonalFinanceDbContext>();
                DateOnly today = DateOnly.FromDateTime(DateTime.Now);

                try
                {
                    await ProcessBudgetsAsync(dbContext, today);
                    await ProcessGoalsAsync(dbContext, today);
                    await ProcessAutoSavingAsync(dbContext, today);

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Có lỗi xảy ra khi xử lý các tác vụ ngầm.");
                }
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }

    private async Task ProcessBudgetsAsync(PersonalFinanceDbContext dbContext, DateOnly today)
    {
        var activeBudgets = await dbContext.Budgets
            .Include(b => b.Category)
            .Where(b => b.StartDate <= today && b.EndDate >= today)
            .ToListAsync();

        foreach (var budget in activeBudgets)
        {
            var totalSpent = await dbContext.Transactions
                .Where(t => t.UserId == budget.UserId &&
                            t.CategoryId == budget.CategoryId &&
                            t.TransactionDate >= budget.StartDate &&
                            t.TransactionDate <= budget.EndDate &&
                            t.IsDelete == false)
                .SumAsync(t => (decimal?)t.Amount) ?? 0;

            if (totalSpent > budget.AmountLimit)
            {
                string categoryName = budget.Category?.CategoryName ?? "hạng mục";
                bool alreadyNotified = await dbContext.Notifications.AnyAsync(n =>
                    n.UserId == budget.UserId &&
                    n.Title == "Cảnh báo vượt hạn mức" &&
                    n.Message.Contains(categoryName) &&
                    n.CreatedAt.HasValue && n.CreatedAt.Value.Date >= budget.StartDate.ToDateTime(TimeOnly.MinValue));

                if (!alreadyNotified)
                {
                    dbContext.Notifications.Add(new Notification
                    {
                        UserId = budget.UserId,
                        Title = "Cảnh báo vượt hạn mức",
                        Message = $"Bạn đã chi tiêu {totalSpent:N0}đ cho '{categoryName}', vượt quá hạn mức ({budget.AmountLimit:N0}đ).",
                        Type = "Warning",
                        CreatedAt = DateTime.Now
                    });
                }
            }
        }
    }
    private async Task ProcessGoalsAsync(PersonalFinanceDbContext dbContext, DateOnly today)
    {
        DateOnly deadlineThreshold = today.AddDays(3);
        DateTime threeDaysAgo = DateTime.Now.AddDays(-3);

        var goalsToNotify = await dbContext.Savinggoals
            .Where(g => g.Status == "Active" && g.TargetDate <= deadlineThreshold && g.TargetDate >= today)
            .ToListAsync();

        foreach (var goal in goalsToNotify)
        {
            bool alreadyNotified = await dbContext.Notifications.AnyAsync(n =>
                n.UserId == goal.UserId &&
                n.Message.Contains(goal.GoalName) &&
                n.CreatedAt >= threeDaysAgo);

            if (!alreadyNotified)
            {
                dbContext.Notifications.Add(new Notification
                {
                    UserId = goal.UserId,
                    Title = "Mục tiêu sắp đến hạn",
                    Message = $"Mục tiêu '{goal.GoalName}' của bạn sắp hết hạn ({goal.TargetDate:dd/MM}).",
                    Type = "Warning",
                    CreatedAt = DateTime.Now
                });
            }
        }
    }
    private async Task ProcessAutoSavingAsync(PersonalFinanceDbContext dbContext, DateOnly today)
    {
        var listUser = await dbContext.Users
            .Include(u => u.Usertaxprofile)
            .Include(u => u.Wallets)
            .Where(u => u.Usertaxprofile != null && u.Usertaxprofile.SavingRate > 0)
            .ToListAsync();

        var firstDayOfLastMonth = new DateOnly(today.Year, today.Month, 1).AddMonths(-1);
        var lastDayOfLastMonth = new DateOnly(today.Year, today.Month, 1).AddDays(-1);

        foreach (var user in listUser)
        {
            bool hasNotified = await dbContext.Notifications.AnyAsync(n =>
                n.UserId == user.Id &&
                n.Title.Contains("trích quỹ") &&
                n.CreatedAt!.Value.Month == today.Month &&
                n.CreatedAt!.Value.Year == today.Year);

            if (hasNotified) continue;
            decimal savingRate = (decimal)(user.Usertaxprofile?.SavingRate ?? 0);

            var savingWallet = user.Wallets.FirstOrDefault(w => w.WalletId == user?.Usertaxprofile?.WalletId);
            var expensesWallet = user.Wallets.FirstOrDefault(w => w.IsDefault == true);

            if (savingWallet == null || expensesWallet == null) continue;

            var totalIncome = dbContext.Transactions
                .Where(t => t.UserId == user.Id && t.Category!.Type == "Income"
                        && t.TransactionDate >= firstDayOfLastMonth
                        && t.TransactionDate <= lastDayOfLastMonth
                        && t.IsDelete == false)
                .Sum(t => (decimal?)t.Amount) ?? 0;

            var totalExpense = dbContext.Transactions
                .Where(t => t.UserId == user.Id && t.Category!.Type == "Expense"
                        && t.TransactionDate >= firstDayOfLastMonth
                        && t.TransactionDate <= lastDayOfLastMonth
                        && t.IsDelete == false)
                .Sum(t => (decimal?)t.Amount) ?? 0;

            decimal surplus = totalIncome - totalExpense;
            if (surplus > 0)
            {
                decimal amountToSave = surplus * (savingRate / 100m);

                if (expensesWallet.Balance >= amountToSave)
                {
                    expensesWallet.Balance -= amountToSave;
                    savingWallet.Balance += amountToSave;

                    dbContext.Notifications.Add(new Notification
                    {
                        UserId = user.Id,
                        Title = "Trích quỹ tự động thành công",
                        Message = $"Tháng qua bạn dư {surplus:N0}đ. Đã trích {amountToSave:N0}đ vào quỹ tiết kiệm.",
                        Type = "Success",
                        CreatedAt = DateTime.Now,
                    });
                }
            }
            else
            {
                dbContext.Notifications.Add(new Notification
                {
                    UserId = user.Id,
                    Title = "Không thể trích quỹ",
                    Message = "Tháng qua chi tiêu vượt mức thu nhập, không có tiền dư để trích quỹ.",
                    Type = "Warning",
                    CreatedAt = DateTime.Now,
                });
            }
        }
    }
}