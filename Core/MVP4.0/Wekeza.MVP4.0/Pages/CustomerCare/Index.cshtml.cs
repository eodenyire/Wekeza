using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wekeza.MVP4._0.Services;

namespace Wekeza.MVP4._0.Pages.CustomerCare
{
    [Authorize(Roles = "CustomerCareOfficer")]
    public class IndexModel : PageModel
    {
        private readonly ICustomerCareService _customerCareService;
        
        public string UserName { get; set; } = "Customer Care Officer";
        public string UserInitials { get; set; } = "CC";
        public int NotificationCount { get; set; } = 15;
        
        public string ActiveInquiries { get; set; } = "0";
        public string ResolvedToday { get; set; } = "0";
        public string AvgResponseTime { get; set; } = "N/A";
        public string SatisfactionScore { get; set; } = "4.5/5";

        public IndexModel(ICustomerCareService customerCareService)
        {
            _customerCareService = customerCareService;
        }

        public async Task OnGetAsync()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                UserName = User.Identity.Name ?? "Customer Care Officer";
                var nameParts = UserName.Split(' ');
                UserInitials = nameParts.Length > 1 
                    ? $"{nameParts[0][0]}{nameParts[1][0]}" 
                    : UserName.Length > 1 ? UserName.Substring(0, 2).ToUpper() : "CC";
            }

            // Load real dashboard statistics
            try
            {
                ActiveInquiries = (await _customerCareService.GetActiveInquiriesCountAsync()).ToString();
                ResolvedToday = (await _customerCareService.GetResolvedTodayCountAsync()).ToString();
                AvgResponseTime = await _customerCareService.GetAverageResponseTimeAsync();
                var score = await _customerCareService.GetSatisfactionScoreAsync();
                SatisfactionScore = $"{score}/5";
            }
            catch (Exception)
            {
                // Keep default values if service fails
            }
        }
    }
}
