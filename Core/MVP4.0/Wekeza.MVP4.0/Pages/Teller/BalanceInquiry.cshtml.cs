using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.Teller
{
    [Authorize(Roles = "Teller")]
    public class BalanceInquiryModel : PageModel
    {
        public string UserName { get; set; } = "Teller";
        public string UserInitials { get; set; } = "T";
        public int NotificationCount { get; set; } = 5;

        public List<RecentInquiry> RecentInquiries { get; set; } = new();

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

            RecentInquiries = new List<RecentInquiry>
            {
                new RecentInquiry { AccountNumber = "1001234567", CustomerName = "John Smith", Balance = "$2,500.00", Time = "11:45 AM" },
                new RecentInquiry { AccountNumber = "1001234568", CustomerName = "Jane Doe", Balance = "$5,800.00", Time = "11:30 AM" },
                new RecentInquiry { AccountNumber = "1001234569", CustomerName = "Mike Johnson", Balance = "$1,200.00", Time = "11:15 AM" },
                new RecentInquiry { AccountNumber = "1001234570", CustomerName = "Sarah Wilson", Balance = "$3,400.00", Time = "11:00 AM" }
            };
        }

        public class RecentInquiry
        {
            public string AccountNumber { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string Balance { get; set; } = string.Empty;
            public string Time { get; set; } = string.Empty;
        }
    }
}