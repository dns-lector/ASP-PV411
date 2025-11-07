using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ASP_PV411.Models.Home
{
    public class HomeDemoViewModel
    {
        public String PageTitle { get; set; } = String.Empty;
        public String FormTitle { get; set; } = String.Empty;
        public HomeDemoFormModel? FormModel { get; set; }
        public Dictionary<String,String>? ModelErrors { get; set; }
    }
}
