using HarmonyLib;

namespace CassieReplacement.Patches
{
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
