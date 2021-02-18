using AzureADXNETCoreWebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureADXNETCoreWebApp.Helpers
{
    public interface IDataHelper
    {
        Task<List<StormEvent>> GetStormEvents(string userId, string searchText = null);
        Task<StormEvent> GetStormEvent(string userId, int eventId);
        Task<bool> UpdateStormEvent(string update);
    }
}