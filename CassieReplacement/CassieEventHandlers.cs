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

        /// <summary>
        /// Registers the event handlers.
        /// </summary>
        public void Register()
        {
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance += OnAnnouncingNtfEntrance;
            Exiled.Events.Handlers.Map.AnnouncingChaosEntrance += OnAnnouncingChaosEntrance;
            Exiled.Events.Handlers.Map.AnnouncingScpTermination += OnAnnouncingScpTermination;
        }

        /// <summary>
        /// Unregisters the event handlers.
        /// </summary>
        public void Unregister()
        {
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance -= OnAnnouncingNtfEntrance;
            Exiled.Events.Handlers.Map.AnnouncingChaosEntrance -= OnAnnouncingChaosEntrance;
            Exiled.Events.Handlers.Map.AnnouncingScpTermination -= OnAnnouncingScpTermination;
        }

        private void OnAnnouncingChaosEntrance(AnnouncingChaosEntranceEventArgs e)
        {
            if (!Config.ShouldOverrideAnnouncements || !e.IsAllowed)
            {
                return;
            }

            e.IsAllowed = false;
            Cassie.Clear();

            CassieAnnouncement newAnnouncement = new CassieAnnouncement();

            if (!e.Wave.IsMiniWave)
            {
                newAnnouncement = Config.ChaosWaveAnnouncement;
            }
            else
            {
                newAnnouncement = Config.ChaosMiniAnnouncement;
            }

            newAnnouncement.Announce();
        }

        private void OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs e)
        {
            if (!Config.ShouldOverrideAnnouncements || !e.IsAllowed)
            {
                return;
            }

            e.IsAllowed = false;
            Cassie.Clear();

            CassieAnnouncement newAnnouncement = new CassieAnnouncement();
            if (!e.Wave.IsMiniWave)
            {
                newAnnouncement = Config.NtfWaveAnnouncement;
            }
            else
            {
                newAnnouncement = Config.NtfMiniAnnouncement;
            }

            newAnnouncement = newAnnouncement
                .Replace("{letter}", new CassieAnnouncement($"nato_{e.UnitName[0]}", e.UnitName))
                .Replace("{number}", e.UnitNumber < 10 ? $"0{e.UnitNumber}" : $"{e.UnitNumber}")
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
            string unit = e.DamageHandler.Attacker is null ? string.Empty : e.DamageHandler.AttackerFootprint.UnitName;

            CassieAnnouncement letter = new CassieAnnouncement();
            string number = string.Empty;

            if (!string.IsNullOrWhiteSpace(unit) && unit.Contains('-'))
            {
                letter = new CassieAnnouncement($"nato_{unit.Split('-')[0][0]}", unit.Split('-')[0]);
                number = unit.Split('-')[1];
            }

            // Fucked up conversion.
            if (Enum.TryParse(e.DamageHandler.Type.ToString(), out DamageTypeEnumToInt enum1))
            {
                damageType = (CassieDamageType)(int)enum1;
            }

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
