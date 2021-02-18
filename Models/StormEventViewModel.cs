namespace AzureADXNETCoreWebApp.Models
{
    public class StormEventViewModel
    {
        public string Message { get; set; }
        public string SearchText { get; set; }

        public StormEvent StormEvent { get; set; }
    }
}
