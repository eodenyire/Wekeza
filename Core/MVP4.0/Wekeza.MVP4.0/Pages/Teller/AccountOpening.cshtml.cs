using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.Teller
{
    [Authorize(Roles = "Teller")]
    public class AccountOpeningModel : PageModel
    {
        public string UserName { get; set; } = "Teller";
        public string UserInitials { get; set; } = "T";
        public int NotificationCount { get; set; } = 5;

        public List<RecentAccount> RecentAccounts { get; set; } = new();

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

            RecentAccounts = new List<RecentAccount>
            {
                new RecentAccount { AccountNumber = "1001234573", CustomerName = "Alice Johnson", AccountType = "Savings", Status = "Active", OpenedDate = "Today" },
                new RecentAccount { AccountNumber = "1001234574", CustomerName = "Bob Wilson", AccountType = "Checking", Status = "Pending", OpenedDate = "Today" },
                new RecentAccount { AccountNumber = "1001234575", CustomerName = "Carol Brown", AccountType = "Savings", Status = "Active", OpenedDate = "Yesterday" },
                new RecentAccount { AccountNumber = "1001234576", CustomerName = "Daniel Lee", AccountType = "Business", Status = "Active", OpenedDate = "Yesterday" }
            };
        }

        public async Task<IActionResult> OnPostAsync(string firstName, string lastName, string email, string phone, string idNumber, string accountType, decimal initialDeposit, string address)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(idNumber) || string.IsNullOrEmpty(accountType))
            {
                TempData["ErrorMessage"] = "Please fill in all required fields.";
                return RedirectToPage();
            }

            if (initialDeposit < 100)
            {
                TempData["ErrorMessage"] = "Minimum initial deposit is $100.00.";
                return RedirectToPage();
            }

            // Generate new account number
            var random = new Random();
            var newAccountNumber = $"100123{random.Next(4577, 9999)}";

            // In a real application, this would create the account in database
            TempData["SuccessMessage"] = $"Account {newAccountNumber} created successfully for {firstName} {lastName}!";
            TempData["NewAccountNumber"] = newAccountNumber;
            
            return RedirectToPage();
        }

        public class RecentAccount
        {
            public string AccountNumber { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string AccountType { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string OpenedDate { get; set; } = string.Empty;
        }
    }
}