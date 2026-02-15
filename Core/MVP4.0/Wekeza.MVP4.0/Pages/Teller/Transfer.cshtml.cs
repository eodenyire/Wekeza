using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.Teller
{
    [Authorize(Roles = "Teller")]
    public class TransferModel : PageModel
    {
        public string UserName { get; set; } = "Teller";
        public string UserInitials { get; set; } = "T";
        public int NotificationCount { get; set; } = 5;

        public List<RecentTransfer> RecentTransfers { get; set; } = new();

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

            RecentTransfers = new List<RecentTransfer>
            {
                new RecentTransfer { FromAccount = "1001234567", ToAccount = "1001234568", Amount = "$1,000.00", Time = "11:30 AM", Status = "Completed" },
                new RecentTransfer { FromAccount = "1001234569", ToAccount = "1001234570", Amount = "$500.00", Time = "11:15 AM", Status = "Completed" },
                new RecentTransfer { FromAccount = "1001234571", ToAccount = "1001234572", Amount = "$250.00", Time = "11:00 AM", Status = "Completed" },
                new RecentTransfer { FromAccount = "1001234567", ToAccount = "1001234569", Amount = "$750.00", Time = "10:45 AM", Status = "Completed" }
            };
        }

        public async Task<IActionResult> OnPostAsync(string fromAccount, string toAccount, decimal amount, string transferType, string description)
        {
            if (string.IsNullOrEmpty(fromAccount) || string.IsNullOrEmpty(toAccount) || amount <= 0)
            {
                TempData["ErrorMessage"] = "Please provide valid account numbers and amount.";
                return RedirectToPage();
            }

            if (fromAccount == toAccount)
            {
                TempData["ErrorMessage"] = "Source and destination accounts cannot be the same.";
                return RedirectToPage();
            }

            // In a real application, this would process the transfer in database
            TempData["SuccessMessage"] = $"Transfer of ${amount:F2} from {fromAccount} to {toAccount} processed successfully!";
            
            return RedirectToPage();
        }

        public class RecentTransfer
        {
            public string FromAccount { get; set; } = string.Empty;
            public string ToAccount { get; set; } = string.Empty;
            public string Amount { get; set; } = string.Empty;
            public string Time { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
        }
    }
}