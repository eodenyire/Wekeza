using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.Teller
{
    [Authorize(Roles = "Teller")]
    public class StatementModel : PageModel
    {
        public string UserName { get; set; } = "Teller";
        public string UserInitials { get; set; } = "T";
        public int NotificationCount { get; set; } = 5;

        public List<RecentStatement> RecentStatements { get; set; } = new();

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

            RecentStatements = new List<RecentStatement>
            {
                new RecentStatement { AccountNumber = "1001234567", CustomerName = "John Smith", Period = "Dec 2025", Status = "Generated", Time = "12:30 PM" },
                new RecentStatement { AccountNumber = "1001234568", CustomerName = "Jane Doe", Period = "Dec 2025", Status = "Emailed", Time = "12:15 PM" },
                new RecentStatement { AccountNumber = "1001234569", CustomerName = "Mike Johnson", Period = "Nov 2025", Status = "Printed", Time = "12:00 PM" },
                new RecentStatement { AccountNumber = "1001234570", CustomerName = "Sarah Wilson", Period = "Dec 2025", Status = "Generated", Time = "11:45 AM" }
            };
        }

        public class RecentStatement
        {
            public string AccountNumber { get; set; } = string.Empty;
            public string CustomerName { get; set; } = string.Empty;
            public string Period { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string Time { get; set; } = string.Empty;
        }
    }
}