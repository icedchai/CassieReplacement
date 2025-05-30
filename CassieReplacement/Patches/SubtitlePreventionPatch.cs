namespace CassieReplacement.Patches
{
    using HarmonyLib;
    using Respawning.Announcements;
    using System.Collections.Generic;
    using System.Reflection;

    [HarmonyPatch]
    public static class SubtitlePreventionPatch
    {
        public static bool Prefix()
        {
            if (Plugin.Singleton.Config.CassieOverrideConfig.ShouldOverrideAnnouncements)
            {
                return false;
            }

            return true;
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            string sendSubtitles = nameof(WaveAnnouncementBase.SendSubtitles);
            yield return typeof(NtfWaveAnnouncement).GetMethod(sendSubtitles);
            yield return typeof(NtfMiniwaveAnnouncement).GetMethod(sendSubtitles);
            yield return typeof(ChaosWaveAnnouncement).GetMethod(sendSubtitles);
            yield return typeof(ChaosMiniwaveAnnouncement).GetMethod(sendSubtitles);
        }
    }
}
