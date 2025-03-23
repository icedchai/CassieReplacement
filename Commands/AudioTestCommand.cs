namespace CassieReplacement.Commands
{
    using CommandSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The command used to invoke <see cref="CommonFuncs.ReadMessage(List{string})(System.Collections.Generic.List{string})"/> in-game.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class AudioTestCommand : ICommand
    {
        /// <inheritdoc/>
        public string Command => "customcassie";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "ccassie", "cassiesay", "custommsg" };

        /// <inheritdoc/>
        public string Description => "Runs a custom CASSIE line.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            List<string> words = arguments.ToList();
            if (float.TryParse(arguments.At(0), out float vol))
            {
                words.Remove(arguments.At(0));
                Plugin.PluginConfig.CassieVolume = vol;
            }

            CommonFuncs.ReadMessage(words);
            response = $"Tried {string.Join(" ", words)}";
            return true;
        }
    }
}
