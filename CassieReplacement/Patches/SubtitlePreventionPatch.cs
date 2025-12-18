/*namespace CassieReplacement.Patches
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
            if (!Plugin.Singleton.Config.CassieOverrideConfig.ShouldOverrideAnnouncements)
            {
                return true;
            }

            return false;
        }

        public static IEnumerable<MethodBase> TargetMethods()
        {
            // TOOD: readd subtitle wave patch
            *//*string sendSubtitles = nameof(WaveAnnouncementBase);
            yield return typeof(NtfWaveAnnouncement).GetMethod(sendSubtitles);
            yield return typeof(NtfMiniwaveAnnouncement).GetMethod(sendSubtitles);
            yield return typeof(ChaosWaveAnnouncement).GetMethod(sendSubtitles);
            yield return typeof(ChaosMiniwaveAnnouncement).GetMethod(sendSubtitles);*//*
        }
    }
}*/
