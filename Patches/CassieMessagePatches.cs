namespace CassieReplacement.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Exiled.API.Features;
    using HarmonyLib;
    using Respawning;

    [HarmonyPatch(typeof(RespawnEffectsController), nameof(RespawnEffectsController.PlayCassieAnnouncement))]
    public static class CassieMessagePatches
    {
        [HarmonyPrefix]
        public static bool MessagePrefix(string words, bool makeHold, bool makeNoise, bool customAnnouncement)
        {
            if (words.StartsWith("customcassie"))
            {
                List<string> input = words.ToLower().Split(' ').ToList();
                input.Remove("customcassie");
                CustomCassieReader.Singleton.CassieReadMessage(input, makeNoise);
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
                List<string> input = message.ToLower().Split(' ').ToList();
                input.Remove("customcassie");
                CustomCassieReader.Singleton.CassieReadMessage(input, isNoisy, translation);
                return false;
            }

            return true;
        }
    }
}
