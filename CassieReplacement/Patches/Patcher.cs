namespace CassieReplacement.Patches
{
    using HarmonyLib;
    using Respawning.Announcements;
    using System.Reflection;

    public static class Patcher
    {
        private static Harmony HarmonyInstance { get; set; }

        /// <summary>
        /// Do patching.
        /// </summary>
        public static void DoPatching()
        {
            HarmonyInstance = new Harmony("me.icedchai.cassie.patch");
            HarmonyInstance.PatchAll();
        }

        /// <summary>
        /// Unpatches.
        /// </summary>
        public static void DoUnpatch()
        {
            HarmonyInstance.UnpatchAll("me.icedchai.cassie.patch");
            HarmonyInstance = null;
        }
    }
}
