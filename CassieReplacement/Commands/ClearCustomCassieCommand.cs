using CommandSystem;
using LabApi.Features.Wrappers;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CassieReplacement.Commands
{
    /// <inheritdoc/>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ClearCustomCassieCommand : ICommand
    {
        /// <inheritdoc/>
        public string Command => "clearcustomcassie";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "customcassieclear", "clearcc", "clearcustom" };

        /// <inheritdoc/>
        public string Description => "Clears custom CASSIE lines.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Cassie.Clear();
            CustomCassieReader.Singleton.ShouldPause = true;
            foreach (var audioPlayer in Plugin.Singleton.CassieAudioPlayers)
            {
                audioPlayer.RemoveAllClips();
            }

            response = "Cleared cassie.";
            return true;
        }
    }
}
