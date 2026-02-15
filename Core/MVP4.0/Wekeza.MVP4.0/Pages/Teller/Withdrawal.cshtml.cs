using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.Teller
{
    [Authorize(Roles = "Teller")]
    public class WithdrawalModel : PageModel
    {
        public string UserName { get; set; } = "Teller";
        public string UserInitials { get; set; } = "T";
        public int NotificationCount { get; set; } = 5;

        public List<RecentTransaction> RecentWithdrawals { get; set; } = new();

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Teller";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "T";
            }

            RecentWithdrawals = new List<RecentTransaction>
            {
                new RecentTransaction { AccountNumber = "1001234567", CustomerName = "John Smith", Amount = "$500.00", Time = "10:45 AM" },
                new RecentTransaction { AccountNumber = "1001234568", CustomerName = "Jane Doe", Amount = "$1,200.00", Time = "10:30 AM" },
                new RecentTransaction { AccountNumber = "1001234569", CustomerName = "Mike Johnson", Amount = "$300.00", Time = "10:15 AM" },
                new RecentTransaction { AccountNumber = "1001234570", CustomerName = "Sarah Wilson", Amount = "$750.00", Time = "10:00 AM" }
            };
        }

        public async Task<IActionResult> OnPostAsync(string accountNumber, string customerName, decimal amount, string withdrawalType, string description)
        {
            if (string.IsNullOrEmpty(accountNumber) || amount <= 0)
            {
                TempData["ErrorMessage"] = "Please provide valid account number and amount.";
                return RedirectToPage();
            }

            // In a real application, this would process the withdrawal in database
            TempData["SuccessMessage"] = $"Withdrawal of ${amount:F2} processed successfully for {customerName}!";
            
            return RedirectToPage();
        }

        public class RecentTransaction
        {
            public string AccountNumber { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string Amount { get; set; } = string.Empty;
            public string Time { get; set; } = string.Empty;
        }
    }
}