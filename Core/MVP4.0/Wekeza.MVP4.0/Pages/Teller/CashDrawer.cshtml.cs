using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.Teller
{
    [Authorize(Roles = "Teller")]
    public class CashDrawerModel : PageModel
    {
        public string UserName { get; set; } = "Teller";
        public string UserInitials { get; set; } = "T";
        public int NotificationCount { get; set; } = 5;

        public string CurrentCashBalance { get; set; } = "25,450.00";
        public string OpeningBalance { get; set; } = "20,000.00";
        public string TotalDeposits { get; set; } = "8,750.00";
        public string TotalWithdrawals { get; set; } = "3,300.00";
        public string DrawerStatus { get; set; } = "Open";

        public List<CashTransaction> CashTransactions { get; set; } = new();

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

            CashTransactions = new List<CashTransaction>
            {
                new CashTransaction { Time = "14:30", Type = "Deposit", Amount = "$500.00", Customer = "John Smith", Reference = "DEP001", Balance = "$25,450.00" },
                new CashTransaction { Time = "14:15", Type = "Withdrawal", Amount = "$200.00", Customer = "Jane Doe", Reference = "WTH001", Balance = "$24,950.00" },
                new CashTransaction { Time = "14:00", Type = "Deposit", Amount = "$1,200.00", Customer = "Mike Johnson", Reference = "DEP002", Balance = "$25,150.00" },
                new CashTransaction { Time = "13:45", Type = "Withdrawal", Amount = "$300.00", Customer = "Sarah Wilson", Reference = "WTH002", Balance = "$23,950.00" },
                new CashTransaction { Time = "13:30", Type = "Deposit", Amount = "$750.00", Customer = "David Brown", Reference = "DEP003", Balance = "$24,250.00" }
            };
        }

        public async Task<IActionResult> OnPostOpenDrawerAsync(decimal openingAmount)
        {
            if (openingAmount <= 0)
            {
                TempData["ErrorMessage"] = "Opening amount must be greater than zero.";
                return RedirectToPage();
            }

            // In a real application, this would update the database
            TempData["SuccessMessage"] = $"Cash drawer opened with ${openingAmount:F2}";
            
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCloseDrawerAsync()
        {
            // In a real application, this would calculate final balance and close drawer
            TempData["SuccessMessage"] = "Cash drawer closed successfully. Reconciliation report generated.";
            
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddCashAsync(decimal amount, string reason)
        {
            if (amount <= 0)
            {
                TempData["ErrorMessage"] = "Amount must be greater than zero.";
                return RedirectToPage();
            }

            // In a real application, this would add cash to drawer
            TempData["SuccessMessage"] = $"${amount:F2} added to cash drawer. Reason: {reason}";
            
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveCashAsync(decimal amount, string reason)
        {
            if (amount <= 0)
            {
                TempData["ErrorMessage"] = "Amount must be greater than zero.";
                return RedirectToPage();
            }

            // In a real application, this would remove cash from drawer
            TempData["SuccessMessage"] = $"${amount:F2} removed from cash drawer. Reason: {reason}";
            
            return RedirectToPage();
        }

        public class CashTransaction
        {
            public string Time { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Amount { get; set; } = string.Empty;
            public string Customer { get; set; } = string.Empty;
            public string Reference { get; set; } = string.Empty;
            public string Balance { get; set; } = string.Empty;
        }
    }
}