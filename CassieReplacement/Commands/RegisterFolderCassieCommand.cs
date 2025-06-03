using CassieReplacement.Models;
using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CassieReplacement.Commands
{
    /// <inheritdoc/>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class RegisterFolderCassieCommand : ICommand
    {
        /// <inheritdoc/>
        public string Command => "registercassie";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "customcassieregister", "registercc", "register" };

        /// <inheritdoc/>
        public string Description => "Registers a specific folder of CASSIE lines. (usage: register (path) (bleed) (prefix)";

        /// <inheritdoc/>w
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = "Not enough arguments.";
                return false;
            }

            string path = arguments.At(0);
            float bleedTime = 0f;
            float.TryParse(arguments.Count > 1 ? arguments.At(1) : "0", out bleedTime);
            string prefix = arguments.Count > 2 ? arguments.At(2) : string.Empty;
            CassieDirectory cassieDirectory = new CassieDirectory() { Path = path, BleedTime = bleedTime, Prefix = prefix };
            Plugin.RegisterFolder(cassieDirectory);
            response = $"Registered cassie directory, path {path}, prefix {prefix}, bleed {bleedTime}";
            return true;
        }
    }
}
