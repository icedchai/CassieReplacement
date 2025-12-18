namespace CassieReplacement
{
    using System;
    using System.Linq;
    using CassieReplacement.Config;
    using CassieReplacement.Reader;
    using PlayerStatsSystem;
    using PlayerRoles;
    using CassieReplacement.Reader.Enums;
#if EXILED
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Map;
#endif

    public class CassieEventHandlers
    {
        private static CassieOverrideConfigs Config => Plugin.Singleton.Config.CassieOverrideConfig;

        public static void HandleAnnouncingWaveEntrance(Faction faction, bool isMiniWave, string unitLetter = "", int unitNumber = 0)
        {
            CustomCassieAnnouncement newAnnouncement = new CustomCassieAnnouncement();
            char unitLetterFirst = 'a';

            if (!string.IsNullOrWhiteSpace(unitLetter))
            {
                unitLetterFirst = unitLetter[0];
            }

            switch (faction)
            {
                case Faction.FoundationStaff:
                    newAnnouncement = isMiniWave ? Config.NtfMiniAnnouncement : Config.NtfWaveAnnouncement;
                    break;
                case Faction.FoundationEnemy:
                    newAnnouncement = isMiniWave ? Config.ChaosMiniAnnouncement : Config.ChaosWaveAnnouncement;
                    break;
            }

            newAnnouncement = newAnnouncement
                .GenericReplacement()
                .Replace("{letter}", new CustomCassieAnnouncement($"nato_{unitLetterFirst}", unitLetter))
                .Replace("{number}", new CustomCassieAnnouncement($"{unitNumber}", unitNumber < 10 ? $"0{unitNumber}" : $"{unitNumber}"));
            newAnnouncement.Announce();
        }

        public static void HandleAnnouncingTermination(DamageHandlerBase damageHandler, RoleTypeId victimRole)
        {
            CustomCassieAnnouncement newAnnouncement = Config.ScpTerminationAnnouncement.GenericReplacement();
            CassieDamageType damageType = CassieDamageType.Unknown;

            RoleTypeId attackerRole = RoleTypeId.None;
            string attackerUnit = string.Empty;

            CustomCassieAnnouncement letter = new CustomCassieAnnouncement();
            CustomCassieAnnouncement number = new CustomCassieAnnouncement();

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

                        if (universalDamageHandler.TranslationId == DeathTranslations.Tesla.Id)
                        {
                            damageType = CassieDamageType.Tesla;
                        }

                        break;
                }
            }
            else
            {
                attackerRole = aDamageHandler.Attacker.Role;
                attackerUnit = aDamageHandler.Attacker.UnitName;
                damageType = CassieDamageType.Player;
            }

            if (!string.IsNullOrWhiteSpace(attackerUnit) && attackerUnit.Contains('-'))
            {
                string[] split = attackerUnit.Split('-');
                string natoLetter = $"nato_{split[0][0]}";
                int natoNumber = int.Parse(split[1]);
                letter = new CustomCassieAnnouncement($"nato_{split[0][0]}", split[0]);
                number = new CustomCassieAnnouncement($"{natoNumber}", natoNumber < 10 ? $"0{natoNumber}" : $"{natoNumber}");
            }

            newAnnouncement = newAnnouncement
                .GenericReplacement()
                .Replace("{scp}", Config.ScpLookupTable[victimRole])
                .Replace("{deathcause}", Config.DamageTypeTerminationAnnouncementLookupTable[damageType])
                .Replace("{team}", Config.TeamTerminationCallsignLookupTable.TryGetValue(attackerRole.GetTeam(), out CustomCassieAnnouncement _callSign) ? _callSign : new CustomCassieAnnouncement())
                .Replace("{scpkiller}", Config.ScpLookupTable.TryGetValue(attackerRole, out _) ? Config.ScpLookupTable[attackerRole] : new CustomCassieAnnouncement())
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
            HandleAnnouncingWaveEntrance(e.Wave.Faction, e.Wave.IsMiniWave, e.UnitName, e.UnitNumber);
        }

        private void OnAnnouncingChaosEntrance(AnnouncingChaosEntranceEventArgs e)
        {
            if (!Config.ShouldOverrideAnnouncements || !e.IsAllowed)
            {
                return;
            }

            e.IsAllowed = false;
            Cassie.Clear();
            HandleAnnouncingWaveEntrance(e.Wave.Faction, e.Wave.IsMiniWave);
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
