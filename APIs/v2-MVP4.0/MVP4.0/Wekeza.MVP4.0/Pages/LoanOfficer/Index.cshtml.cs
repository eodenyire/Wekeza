using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.LoanOfficer
{
    [Authorize(Roles = "LoanOfficer")]
    public class IndexModel : PageModel
    {
        public string UserName { get; set; } = "Loan Officer";
        public string UserInitials { get; set; } = "LO";
        public int NotificationCount { get; set; } = 9;
        
        public string PendingApplications { get; set; } = "23";
        public string ActivePortfolio { get; set; } = "$2.4M";
        public string DisbursementsToday { get; set; } = "5";
        public string OverdueLoans { get; set; } = "7";
        
        public List<LoanApplication> RecentApplications { get; set; } = new();

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Loan Officer";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "LO";
            }

            RecentApplications = new List<LoanApplication>
            {
                new LoanApplication { CustomerName = "John Doe", Amount = "$50,000", Type = "Personal", Status = "Pending", StatusClass = "warning" },
                new LoanApplication { CustomerName = "Jane Smith", Amount = "$150,000", Type = "Mortgage", Status = "Approved", StatusClass = "success" },
                new LoanApplication { CustomerName = "Mike Johnson", Amount = "$25,000", Type = "Auto", Status = "Under Review", StatusClass = "info" }
            };
        }

        public class LoanApplication
        {
            public string CustomerName { get; set; } = string.Empty;
            public string Amount { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string StatusClass { get; set; } = string.Empty;
        }
    }
}
