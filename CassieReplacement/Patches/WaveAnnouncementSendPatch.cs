#if !EXILED
namespace CassieReplacement.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using HarmonyLib;
    using Respawning.Announcements;
    using Respawning.NamingRules;
    using Respawning.Waves;

    [HarmonyPatch]
    public static class WaveAnnouncementSendPatch
    {
        public static bool Prefix(WaveAnnouncementBase waveMessage)
        {
            if (!Plugin.Singleton.Config.CassieOverrideConfig.ShouldOverrideAnnouncements)
            {
                return true;
            }

            string unitLetter = string.Empty;
            int unitNumber = 0;
            if (NamingRulesManager.TryGetNamingRule(PlayerRoles.Team.FoundationForces, out var rule))
            {
                unitLetter = rule.LastGeneratedName.Split('-')[0];
                unitNumber = int.Parse(rule.LastGeneratedName.Split('-')[1]);
            }

            switch (waveMessage)
            {
                case NtfWaveAnnouncement:
                    CassieEventHandlers.HandleAnnouncingWaveEntrance(PlayerRoles.Faction.FoundationStaff, false, unitLetter, unitNumber);
                    break;
                case ChaosWaveAnnouncement:
                    CassieEventHandlers.HandleAnnouncingWaveEntrance(PlayerRoles.Faction.FoundationEnemy, false, unitLetter, unitNumber);
                    break;
                case NtfMiniwaveAnnouncement:
                    CassieEventHandlers.HandleAnnouncingWaveEntrance(PlayerRoles.Faction.FoundationStaff, true, unitLetter, unitNumber);
                    break;
                case ChaosMiniwaveAnnouncement:
                    CassieEventHandlers.HandleAnnouncingWaveEntrance(PlayerRoles.Faction.FoundationEnemy, true, unitLetter, unitNumber);
                    break;
            }

            return false;
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            string playAnnouncement = nameof(WaveAnnouncementBase.PlayAnnouncement);
            yield return typeof(NtfWaveAnnouncement).GetMethod(playAnnouncement);
            yield return typeof(NtfMiniwaveAnnouncement).GetMethod(playAnnouncement);
            yield return typeof(ChaosWaveAnnouncement).GetMethod(playAnnouncement);
            yield return typeof(ChaosMiniwaveAnnouncement).GetMethod(playAnnouncement);
        }
    }
}
#endif