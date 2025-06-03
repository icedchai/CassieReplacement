namespace CassieReplacement.Commands
{
    using CommandSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The command used to invoke <see cref="CustomCassieReader.ReadMessage(List{string})(System.Collections.Generic.List{string})"/> in-game.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SetAudioVolumeCommand : ICommand
    {
        /// <inheritdoc/>
        public string Command => "customcassievolume";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "cassievolume", "ccassievolume", "ccvolume" };

        /// <inheritdoc/>
        public string Description => "Sets the volume of the custom CASSIE speakers.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            List<string> words = arguments.ToList();

            string firstarg = words.First();
            if (float.TryParse(firstarg, out float vol))
            {
                Plugin.Singleton.Config.CassieVolume = vol;
                response = $"CASSIE volume set to {vol}";
                return true;
            }

            response = "No volume provided.";
            return false;
        }
    }
}
