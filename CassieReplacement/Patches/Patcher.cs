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
        }
    }
}
