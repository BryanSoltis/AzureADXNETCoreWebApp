using System.Collections.Generic;

namespace AzureADXNETCoreWebApp.Models
{
    public class StormEventsViewModel
    {
        public string Message { get; set; }
        public string SearchText { get; set; }

        public List<StormEvent> StormEvents { get; set; }
    }
}
