namespace CassieReplacement.Patches
{
    using Exiled.API.Features;
    using HarmonyLib;
    using Respawning;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [HarmonyPatch(typeof(RespawnEffectsController), nameof(RespawnEffectsController.PlayCassieAnnouncement))]
    public static class CassieMessagePatches
    {
        [HarmonyPrefix]
        public static bool MessagePrefix(string words, bool makeHold, bool makeNoise, bool customAnnouncement)
        {
            if (words.StartsWith("customcassie"))
            {
                CommonFuncs.ReadMessage(words.Split(' ').ToList());
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
