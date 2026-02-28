using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.Teller
{
    [Authorize(Roles = "Teller")]
    public class IndexModel : PageModel
    {
        public string UserName { get; set; } = "Teller User";
        public string UserInitials { get; set; } = "TU";
        public int NotificationCount { get; set; } = 8;
        
        // Dashboard Metrics
        public string TodayTransactions { get; set; } = "47";
        public string CashDrawerBalance { get; set; } = "$25,450";
        public string PendingApprovals { get; set; } = "3";
        public string TotalAmount { get; set; } = "$142.5K";
        
        public List<Transaction> PendingTransactions { get; set; } = new();
        public List<Transaction> RecentTransactions { get; set; } = new();

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Teller User";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "TU";
            }

            PendingTransactions = new List<Transaction>
            {
                new Transaction { AccountNumber = "1001234567", Type = "Withdrawal", Amount = "$5,000", Status = "Pending" },
                new Transaction { AccountNumber = "1001234568", Type = "Transfer", Amount = "$2,500", Status = "Pending" },
                new Transaction { AccountNumber = "1001234569", Type = "Deposit", Amount = "$10,000", Status = "Pending" }
            };

            RecentTransactions = new List<Transaction>
            {
                new Transaction { Time = "10:45 AM", AccountNumber = "1001234567", Type = "Deposit", Amount = "$1,500" },
                new Transaction { Time = "10:30 AM", AccountNumber = "1001234568", Type = "Withdrawal", Amount = "$800" },
                new Transaction { Time = "10:15 AM", AccountNumber = "1001234569", Type = "Transfer", Amount = "$2,000" },
                new Transaction { Time = "10:00 AM", AccountNumber = "1001234570", Type = "Deposit", Amount = "$5,000" },
                new Transaction { Time = "09:45 AM", AccountNumber = "1001234571", Type = "Withdrawal", Amount = "$300" }
            };
        }

        public class Transaction
        {
            public string Time { get; set; } = string.Empty;
            public string AccountNumber { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Amount { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
        }
    }
}
