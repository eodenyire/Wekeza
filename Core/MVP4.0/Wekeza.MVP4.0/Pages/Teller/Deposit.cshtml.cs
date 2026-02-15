using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.Teller
{
    [Authorize(Roles = "Teller")]
    public class DepositModel : PageModel
    {
        public string UserName { get; set; } = "Teller User";
        public string UserInitials { get; set; } = "TU";
        public int NotificationCount { get; set; } = 8;

        public List<RecentDeposit> RecentDeposits { get; set; } = new();

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

            RecentDeposits = new List<RecentDeposit>
            {
                new RecentDeposit { AccountNumber = "1001234567", CustomerName = "John Smith", Amount = "$1,500.00", Time = "10:45 AM" },
                new RecentDeposit { AccountNumber = "1001234568", CustomerName = "Jane Doe", Amount = "$2,300.00", Time = "10:30 AM" },
                new RecentDeposit { AccountNumber = "1001234569", CustomerName = "Mike Johnson", Amount = "$850.00", Time = "10:15 AM" },
                new RecentDeposit { AccountNumber = "1001234570", CustomerName = "Sarah Wilson", Amount = "$5,000.00", Time = "10:00 AM" }
            };
        }

        public class RecentDeposit
        {
            public string AccountNumber { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string Amount { get; set; } = string.Empty;
            public string Time { get; set; } = string.Empty;
        }
    }
}