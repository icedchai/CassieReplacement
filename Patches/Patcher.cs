using Exiled.API.Features;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CassieReplacement.Patches
{
    public static class Patcher
    {
        /// <summary>
        /// Do patching.
        /// </summary>
        public static void DoPatching()
        {
            Log.Debug("patched");
            var harmony = new Harmony("me.icedchai.cassie.patch");
            harmony.PatchAll();
        }
    }
}
