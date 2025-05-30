namespace CassieReplacement
{
    using System;
    using System.Linq;
    using CassieReplacement.Models;
    using CassieReplacement.Models.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Map;
    using PlayerRoles;
    using PlayerStatsSystem;
    using Respawning.Waves;

    public class CassieEventHandlers
    {
        private CassieOverrideConfigs Config => Plugin.Singleton.Config.CassieOverrideConfig;

        public void HandleWaveEntrance(TimeBasedWave wave, string unitName)
        {
            string unitLetter = unitName.Split('-')[0];
            int unitNumber = int.Parse(unitName.Split('-')[1]);
            CassieAnnouncement newAnnouncement = new CassieAnnouncement();
            if (wave is not IMiniWave)
            {
                newAnnouncement = Config.NtfWaveAnnouncement;
            }
            else
            {
                newAnnouncement = Config.NtfMiniAnnouncement;
            }

            newAnnouncement = newAnnouncement
                .GenericReplacement()
                .Replace("{letter}", new CassieAnnouncement($"nato_{unitLetter[0]}", unitName))
                .Replace("{number}", new CassieAnnouncement($"{unitNumber}", unitNumber < 10 ? $"0{unitNumber}" : $"{unitNumber}"));
            newAnnouncement.Announce();
        }

        public void HandleAnnouncingTermination(DamageHandlerBase damageHandler, RoleTypeId victimRole)
        {
            CassieAnnouncement newAnnouncement = Config.ScpTerminationAnnouncement.GenericReplacement();
            CassieDamageType damageType = CassieDamageType.Unknown;

            RoleTypeId attackerRole = RoleTypeId.None;
            string attackerUnit = string.Empty;

            CassieAnnouncement letter = new CassieAnnouncement();
            CassieAnnouncement number = new CassieAnnouncement();

            if (damageHandler is not AttackerDamageHandler aDamageHandler)
            {
                switch (damageHandler)
                {
                    case WarheadDamageHandler:
                        damageType = CassieDamageType.Warhead;
                        break;
                    case UniversalDamageHandler universalDamageHandler:
                        if (universalDamageHandler.TranslationId == DeathTranslations.Decontamination.Id)
                        {
                            damageType = CassieDamageType.Decontamination;
                        }

                        break;
                }
            }
            else
            {
                attackerRole = aDamageHandler.Attacker.Role;
                attackerUnit = aDamageHandler.Attacker.UnitName;
            }

            if (!string.IsNullOrWhiteSpace(attackerUnit) && attackerUnit.Contains('-'))
            {
                string[] split = attackerUnit.Split('-');
                string natoLetter = $"nato_{split[0][0]}";
                int natoNumber = int.Parse(split[1]);
                letter = new CassieAnnouncement($"nato_{split[0][0]}", split[0]);
                number = new CassieAnnouncement($"{natoNumber}", natoNumber < 10 ? $"0{natoNumber}" : $"{natoNumber}");
            }

            newAnnouncement = newAnnouncement
                .GenericReplacement()
                .Replace("{scp}", Config.ScpLookupTable[victimRole])
                .Replace("{deathcause}", Config.DamageTypeTerminationAnnouncementLookupTable[damageType])
                .Replace("{team}", Config.TeamTerminationCallsignLookupTable[attackerRole.GetTeam()])
                .Replace("{scpkiller}", Config.ScpLookupTable.TryGetValue(attackerRole, out _) ? Config.ScpLookupTable[attackerRole] : new CassieAnnouncement())
                .Replace("{letter}", letter)
                .Replace("{number}", number);
            newAnnouncement.Announce();
        }

#if EXILED
        private void OnAnnouncingNtfEntrance(AnnouncingNtfEntranceEventArgs e)
        {
            if (!Config.ShouldOverrideAnnouncements || !e.IsAllowed)
            {
                return;
            }

            e.IsAllowed = false;
            Cassie.Clear();
        }

        private void OnAnnouncingChaosEntrance(AnnouncingChaosEntranceEventArgs e)
        {
            if (!Config.ShouldOverrideAnnouncements || !e.IsAllowed)
            {
                return;
            }

            e.IsAllowed = false;
            Cassie.Clear();
        }

        private void OnAnnouncingScpTermination(AnnouncingScpTerminationEventArgs e)
        {
            if (!Config.ShouldOverrideAnnouncements || !e.IsAllowed)
            {
                return;
            }

            e.IsAllowed = false;
            HandleAnnouncingTermination(e.DamageHandler.Base, e.Role);
        }

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
#endif
    }
}
