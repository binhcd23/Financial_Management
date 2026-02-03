using Financial_Management_Server.Models;
using System.ComponentModel.DataAnnotations;

namespace Financial_Management_Server.DTOs.Finances
{
    public class GoalDto
    {
        public int GoalId { get; set; }

        [Required]
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Tên mục tiêu không được để trống")]
        [StringLength(100, ErrorMessage = "Tên mục tiêu không quá 100 ký tự")]
        public string GoalName { get; set; } = null!;

        [Required(ErrorMessage = "Số tiền mục tiêu không được để trống")]
        [Range(1000, double.MaxValue, ErrorMessage = "Số tiền mục tiêu phải lớn hơn 1,000")]
        public decimal TargetAmount { get; set; }

        public decimal? CurrentAmount { get; set; }

        public double ProgressPercentage => TargetAmount > 0
          ? Math.Round((double)((CurrentAmount ?? 0) / TargetAmount * 100), 1)
          : 0;

        public DateOnly? StartDate { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        public DateOnly? TargetDate { get; set; }

        public string TimeLeftText => DaysLeft.HasValue ? $"{DaysLeft} ngày" : "Vô thời hạn";

        public string? Status { get; set; }

        public GoalDto() { }
        public GoalDto(Savinggoal goal)
        {
            GoalId = goal.GoalId;
            UserId = goal.UserId;
            GoalName = goal.GoalName;
            TargetAmount = goal.TargetAmount;
            CurrentAmount = goal.CurrentAmount;
            StartDate = goal.StartDate;
            TargetDate = goal.TargetDate;
            Status = goal.Status;
        }

        public Savinggoal ToGoal()
        {
            return new Savinggoal
            {
                UserId = UserId,
                GoalName = GoalName,
                TargetAmount = TargetAmount,
                CurrentAmount = CurrentAmount ?? 0,
                StartDate = StartDate ?? DateOnly.FromDateTime(DateTime.Now),
                TargetDate = TargetDate,
                Status = "Active",
            };
        }
        public int? DaysLeft
        {
            get
            {
                if (!TargetDate.HasValue) return null;

                var today = DateOnly.FromDateTime(DateTime.Now);
                var diff = TargetDate.Value.DayNumber - today.DayNumber;

                return diff > 0 ? diff : 0;
            }
        }
    }
    public class GoalRequestDto
    {
        public int userId { get; set; }
        public string? search { get; set; }
        public string? status { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
    }
    public class RequestedValue
    {
        [Required]
        public int goalId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tiền muốn thêm")]
        [Range(1000, double.MaxValue, ErrorMessage = "Số tiền thêm vào tối thiểu phải là 1,000")]
        public decimal addedValue { get; set; }
    }
    public class GoalResponses
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
