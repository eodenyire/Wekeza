using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Wekeza.MVP4._0.Pages.CashOffice
{
    [Authorize(Roles = "CashOfficer")]
    public class IndexModel : PageModel
    {
        public string UserName { get; set; } = "Cash Officer";
        public string UserInitials { get; set; } = "CO";
        public int NotificationCount { get; set; } = 4;
        
        public string VaultBalance { get; set; } = "$2.8M";
        public string TellerFloat { get; set; } = "$240K";
        public string MovementsToday { get; set; } = "32";
        public string PendingRequests { get; set; } = "6";
        
        public List<CashMovement> CashMovements { get; set; } = new();
        public List<TellerFloat> TellerFloatStatus { get; set; } = new();

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Cash Officer";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "CO";
            }

            CashMovements = new List<CashMovement>
            {
                new CashMovement { Time = "09:00 AM", Type = "Float Issue", Amount = "$30,000", Teller = "John Doe" },
                new CashMovement { Time = "10:30 AM", Type = "Float Issue", Amount = "$30,000", Teller = "Jane Smith" },
                new CashMovement { Time = "02:15 PM", Type = "Vault Deposit", Amount = "$50,000", Teller = "System" }
            };

            TellerFloatStatus = new List<TellerFloat>
            {
                new TellerFloat { Name = "John Doe", Amount = "$30,000", Status = "Active", StatusClass = "success" },
                new TellerFloat { Name = "Jane Smith", Amount = "$30,000", Status = "Active", StatusClass = "success" },
                new TellerFloat { Name = "Mike Johnson", Amount = "$30,000", Status = "Reconciled", StatusClass = "info" }
            };
        }

        public class CashMovement
        {
            public string Time { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Amount { get; set; } = string.Empty;
            public string Teller { get; set; } = string.Empty;
        }

        public class TellerFloat
        {
            public string Name { get; set; } = string.Empty;
            public string Amount { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string StatusClass { get; set; } = string.Empty;
        }
    }
}
