#if EXILED
namespace CassieReplacement
{
    using CassieReplacement.Models;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Map;
    using LabApi.Events.Arguments.ServerEvents;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Policy;
    using System.Text;
    using System.Threading.Tasks;

    public class CassieEventHandlers
    {
        private CassieOverrideConfigs Config => Plugin.PluginConfig.CassieOverrideConfig;
/*
        public void Register()
        {
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance += OnAnnouncingNtfEntrance;
        }

        public void Unregister()
        {
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance -= OnAnnouncingNtfEntrance;
        }

        private void OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs e)
        {
            e.IsAllowed = false;
            Log.Info($"{e.UnitName}, {e.UnitNumber}");
            CassieAnnouncement newAnnouncement = Config.NtfWaveAnnouncement;
            newAnnouncement = newAnnouncement
                .Replace("{letter}", new CassieAnnouncement($"nato_{e.UnitName[0]}", e.UnitName))
                .Replace("{threatoverview}", e.ScpsLeft == 0 ? Config.ThreatOverviewNoScps : e.ScpsLeft == 1 ? Config.ThreatOverviewOneScp : Config.ThreatOverviewScps)
                .Replace("{scps}", $"{e.ScpsLeft}");
            newAnnouncement.Announce();
        }*/
    }
}
#endif
