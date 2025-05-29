namespace CassieReplacement.Patches
{
    using HarmonyLib;
    using Respawning.Announcements;

    public static class SubtitlePreventionPatch
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            if (Plugin.Singleton.Config.CassieOverrideConfig.ShouldOverrideAnnouncements)
            {
                return false;
            }

            return true;
        }
    }
}
