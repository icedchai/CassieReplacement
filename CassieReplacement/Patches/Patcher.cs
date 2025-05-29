namespace CassieReplacement.Patches
{
    using HarmonyLib;
    using Respawning.Announcements;
    using System.Reflection;

    public static class Patcher
    {
        /// <summary>
        /// Do patching.
        /// </summary>
        public static void DoPatching()
        {
            var harmony = new Harmony("me.icedchai.cassie.patch");
            harmony.PatchAll();

            string sendSubtitles = nameof(WaveAnnouncementBase.SendSubtitles);
            MethodBase ntfWave = typeof(NtfWaveAnnouncement).GetMethod(sendSubtitles);
            MethodBase ntfMiniWave = typeof(NtfMiniwaveAnnouncement).GetMethod(sendSubtitles);
            MethodBase chaosWave = typeof(ChaosWaveAnnouncement).GetMethod(sendSubtitles);
            MethodBase chaosMiniWave = typeof(ChaosMiniwaveAnnouncement).GetMethod(sendSubtitles);

            MethodInfo subtitlePreventionPatchPrefix = AccessTools.Method(typeof(SubtitlePreventionPatch), nameof(SubtitlePreventionPatch.Prefix));

            harmony.Patch(ntfWave, prefix: new HarmonyMethod(subtitlePreventionPatchPrefix));
            harmony.Patch(ntfMiniWave, prefix: new HarmonyMethod(subtitlePreventionPatchPrefix));
            harmony.Patch(chaosWave, prefix: new HarmonyMethod(subtitlePreventionPatchPrefix));
            harmony.Patch(chaosMiniWave, prefix: new HarmonyMethod(subtitlePreventionPatchPrefix));
        }
    }
}
