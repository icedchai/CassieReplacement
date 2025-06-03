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
    public class UnregisterCassieCommand : ICommand
    {
        /// <inheritdoc/>
        public string Command => "unregistercassie";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "customcassieunregister", "unregistercc", "unregister" };

        /// <inheritdoc/>
        public string Description => "De-registers all custom CASSIE lines.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Plugin.UnregisterClips();
            response = "Unregistered clips.";
            return true;
        }
    }
}
