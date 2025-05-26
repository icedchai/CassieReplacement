#if EXILED
namespace CassieReplacement
{
    using CassieReplacement.Models;
    using CassieReplacement.Models.Enums;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Map;
    using LabApi.Events.Arguments.ServerEvents;
    using PlayerRoles;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Policy;
    using System.Text;
    using System.Threading.Tasks;

    public class CassieEventHandlers
    {
        private CassieOverrideConfigs Config => Plugin.PluginConfig.CassieOverrideConfig;

        public void Register()
        {
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance += OnAnnouncingNtfEntrance;
            Exiled.Events.Handlers.Map.AnnouncingScpTermination += OnAnnouncingScpTermination;
        }

        public void Unregister()
        {
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance -= OnAnnouncingNtfEntrance;
            Exiled.Events.Handlers.Map.AnnouncingScpTermination -= OnAnnouncingScpTermination;
        }

        private void OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs e)
        {
            if (!Config.ShouldOverrideAnnouncements)
            {
                return;
            }

            e.IsAllowed = false;

            Log.Info($"{e.UnitName}, {e.UnitNumber}");
            CassieAnnouncement newAnnouncement = Config.NtfWaveAnnouncement;
            newAnnouncement = newAnnouncement
                .Replace("{letter}", new CassieAnnouncement($"nato_{e.UnitName[0]}", e.UnitName))
                .Replace("{number}", $"{e.UnitNumber}")
                .Replace("{threatoverview}", e.ScpsLeft == 0 ? Config.ThreatOverviewNoScps : e.ScpsLeft == 1 ? Config.ThreatOverviewOneScp : Config.ThreatOverviewScps)
                .Replace("{scps}", $"{e.ScpsLeft}");
            newAnnouncement.Announce();
        }

        private void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs e)
        {
            if (!Config.ShouldOverrideAnnouncements)
            {
                return;
            }

            e.IsAllowed = false;

            CassieAnnouncement newAnnouncement = Config.ScpTerminationAnnouncement;
            CassieDamageType damageType = CassieDamageType.Unknown;

            RoleTypeId role = e.DamageHandler.AttackerFootprint.Role;
            string unit = e.Attacker is null ? string.Empty : e.Attacker.UnitName;

            CassieAnnouncement letter = new CassieAnnouncement();
            string number = string.Empty;

            if (!string.IsNullOrWhiteSpace(unit) && unit.Contains('-'))
            {
                letter = new CassieAnnouncement($"nato_{unit.Split('-')[0]}", unit.Split('-')[0]);
                number = unit.Split('-')[1];
            }

            Enum.TryParse(e.DamageHandler.Type.ToString(), out damageType);

            newAnnouncement = newAnnouncement
                .Replace("{scp}", Config.ScpLookupTable[e.Role.Type])
                .Replace("{deathcause}", Config.DamageTypeTerminationAnnouncementLookupTable[damageType])
                .Replace("{team}", Config.TeamTerminationCallsignLookupTable[role.GetTeam()])
                .Replace("{scpkiller}", Config.ScpLookupTable.TryGetValue(role, out _) ? Config.ScpLookupTable[role] : new CassieAnnouncement())
                .Replace("{letter}", letter)
                .Replace("{number}", number);
            newAnnouncement.Announce();
        }
    }
}
#endif
