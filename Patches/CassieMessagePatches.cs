namespace CassieReplacement.Patches
{
    using Exiled.API.Features;
    using HarmonyLib;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [HarmonyPatch(typeof(Cassie), nameof(Cassie.Message))]
    public static class CassieMessagePatches
    {
        [HarmonyPrefix]
        public static bool MessagePrefix(string message, bool isHeld, bool isNoisy, bool isSubtitles)
        {
            if (message.StartsWith("customcassie"))
            {
                CommonFuncs.ReadMessage(message.Split(' ').ToList());
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(Cassie), nameof(Cassie.MessageTranslated))]
        [HarmonyPrefix]
        public static bool TranslatedMessagePrefix(string message, string translation, bool isHeld, bool isNoisy, bool isSubtitles)
        {
            if (message.StartsWith("customcassie"))
            {
                CommonFuncs.ReadMessage(message.Split(' ').ToList(), translation);
                return false;
            }

            return true;
        }
    }
}
