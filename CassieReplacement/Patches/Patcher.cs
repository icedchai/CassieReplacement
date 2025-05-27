using HarmonyLib;
using LabApi.Features.Console;
using LabApi.Loader.Features.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var harmony = new Harmony("me.icedchai.cassie.patch");
            harmony.PatchAll();
        }
    }
}
