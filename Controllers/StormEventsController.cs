using AzureADXNETCoreWebApp.Helpers;
using AzureADXNETCoreWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureADXNETCoreWebApp.Controllers
{
    public class StormEventsController : Controller
    {
        private IMemoryCache _cache;
        private IDataHelper _dataHelper;

        public StormEventsController(IMemoryCache memoryCache, IDataHelper dataHelper)
        {
            _dataHelper = dataHelper;
            _cache = memoryCache;
        }

        // GET: StormEventsController
        [Authorize]
        public ActionResult Index(string searchText = null)
        {
            var data = new StormEventsViewModel();
            try
            {
                if (searchText != null)
                {
                    data.SearchText = searchText;
                }
                else
                {
                    data.SearchText = "";
                }

                List<StormEvent> lstStormEvents;

                bool isExist = _cache.TryGetValue("AllStormEvents" + searchText, out lstStormEvents);
                if (!isExist)
                {
                    lstStormEvents = _dataHelper.GetStormEvents(User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value, searchText).Result;
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(1000));

                    if (lstStormEvents.Count > 0)
                    {
                        _cache.Set("AllStormEvents" + searchText, lstStormEvents, cacheEntryOptions);
                    }
                    else
                    {
                        data.Message = "No records found.";
                    }
                }
                else
                {
                    lstStormEvents = (List<StormEvent>)_cache.Get("AllStormEvents" + searchText);
                }

                data.StormEvents = lstStormEvents;

            }
            catch (Exception ex)
            {
                data.Message = ex.Message;
            }
            return View(data);
        }

        // GET: StormEventsController/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            var data = new StormEventViewModel();
            try
            {
                StormEvent stormevent;
                bool isExist = _cache.TryGetValue("StormEvent" + id, out stormevent);
                if (!isExist)
                {
                    stormevent = _dataHelper.GetStormEvent(User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value, id).Result;
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(1000));

                    if (stormevent != null)
                    {
                        _cache.Set("StormEvent" + id, stormevent, cacheEntryOptions);
                    }
                    else
                    {
                        data.Message = "There was an error loading the record.";
                    }
                }
                else
                {
                    stormevent = (StormEvent)_cache.Get("StormEvent" + id);
                }
                data.StormEvent = stormevent;
            }
            catch (Exception ex)
            {
                data.Message = ex.Message;
            }

            return View(data);
        }

        // GET: StormEventsController/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            var data = new StormEventViewModel();
            try
            {
                StormEvent stormevent;
                bool isExist = _cache.TryGetValue("StormEvent" + id, out stormevent);
                if (!isExist)
                {
                    stormevent = _dataHelper.GetStormEvent(User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value, id).Result;
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(1000));
                    if (stormevent != null)
                    {
                        _cache.Set("StormEvent" + id, stormevent, cacheEntryOptions);
                    }
                    else
                    {
                        data.Message = "There was an error loading the record.";
                    }
                }
                else
                {
                    stormevent = (StormEvent)_cache.Get("StormEvent" + id);
                }

                data.StormEvent = stormevent;
            }
            catch (Exception ex)
            {
                data.Message = ex.Message;
            }
            return View(data);
        }

        // POST: StormEventsController/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(StormEvent stormevent)
        {
            var data = new StormEventViewModel();
            data.StormEvent = stormevent;
            try
            {
                if (await _dataHelper.UpdateStormEvent(JsonConvert.SerializeObject(stormevent)))
                {
                    data.Message = "Storm Event updated!";
                }
                else
                {
                    data.Message = "There was an error updating the data.";
                }
            }
            catch (Exception ex)
            {
                data.Message = ex.Message;
            }

            // Clear the cached data
            _cache.Remove("StormEvent" + stormevent.EventId);
            _cache.Remove("AllStormEvents");

            return View(data);
        }
    }
}
