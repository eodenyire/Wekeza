using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.Supervisor
{
    [Authorize(Roles = "Supervisor")]
    public class IndexModel : PageModel
    {
        public string UserName { get; set; } = "Supervisor User";
        public string UserInitials { get; set; } = "SU";
        public int NotificationCount { get; set; } = 12;
        
        public string PendingApprovals { get; set; } = "15";
        public string TeamPerformance { get; set; } = "94%";
        public string ActiveCases { get; set; } = "8";
        public string Exceptions { get; set; } = "4";
        
        public List<Approval> PendingApprovalsList { get; set; } = new();
        public List<TeamMember> TeamMembers { get; set; } = new();

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Supervisor User";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "SU";
            }

            PendingApprovalsList = new List<Approval>
            {
                new Approval { Type = "Withdrawal", Teller = "John Doe", Amount = "$15,000" },
                new Approval { Type = "Transfer", Teller = "Jane Smith", Amount = "$25,000" },
                new Approval { Type = "Loan Disbursement", Teller = "Mike Johnson", Amount = "$50,000" }
            };

            TeamMembers = new List<TeamMember>
            {
                new TeamMember { Name = "John Doe", Transactions = "42", Performance = "Excellent", PerformanceClass = "success" },
                new TeamMember { Name = "Jane Smith", Transactions = "38", Performance = "Good", PerformanceClass = "primary" },
                new TeamMember { Name = "Mike Johnson", Transactions = "35", Performance = "Average", PerformanceClass = "warning" }
            };
        }

        public class Approval
        {
            public string Type { get; set; } = string.Empty;
            public string Teller { get; set; } = string.Empty;
            public string Amount { get; set; } = string.Empty;
        }

        public class TeamMember
        {
            public string Name { get; set; } = string.Empty;
            public string Transactions { get; set; } = string.Empty;
            public string Performance { get; set; } = string.Empty;
            public string PerformanceClass { get; set; } = string.Empty;
        }
    }
}
