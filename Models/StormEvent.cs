using System;
using System.ComponentModel.DataAnnotations;
using static AzureADXNETCoreWebApp.Helpers.Attributes;

namespace AzureADXNETCoreWebApp.Models
{
    public class StormEvent
    {
        [DataField("StartTime")]
        public DateTime StartTime { get; set; }

        [DataField("EndTime")]
        public DateTime EndTime { get; set; }

        [DataField("EpisodeId")]
        public int EpisodeId { get; set; }

        [Key]
        [DataField("EventId")]
        public int EventId { get; set; }

        [DataField("State")]
        public string State { get; set; }

        [DataField("EventType")]
        public string EventType { get; set; }

        [DataField("InjuriesDirect")]
        public int InjuriesDirect { get; set; }

        [DataField("InjuriesIndirect")]
        public int InjuriesIndirect { get; set; }

        [DataField("DeathsDirect")]
        public int DeathsDirect { get; set; }

        [DataField("DeathsIndirect")]
        public int DeathsIndirect { get; set; }

        [DataField("DamageProperty")]
        public int DamageProperty { get; set; }

        [DataField("DamageCrops")]
        public int DamageCrops { get; set; }

        [DataField("Source")]
        public string Source { get; set; }

        [DataField("BeginLocation")]
        public string BeginLocation { get; set; }

        [DataField("EndLocation")]
        public string EndLocation { get; set; }

        [DataField("BeginLat")]
        public string BeginLat { get; set; }

        [DataField("BeginLon")]
        public string BeginLon { get; set; }

        [DataField("EndLat")]
        public string EndLat { get; set; }

        [DataField("EndLon")]
        public string EndLon { get; set; }

        [DataField("EpisodeNarrative")]
        public string EpisodeNarrative { get; set; }

        [DataField("EventNarrative")]
        public string EventNarrative { get; set; }

        //[DataField("StormSummary")]
        //public string StormSummary { get; set; }
    }
}
