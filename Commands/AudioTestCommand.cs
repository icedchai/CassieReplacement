namespace CassieReplacement.Commands
{
    using CommandSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static UnityEngine.GraphicsBuffer;

    /// <summary>
    /// The command used to invoke <see cref="Reader.ReadMessage(List{string})(System.Collections.Generic.List{string})"/> in-game.
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

            string firstarg = words.First();
            if (float.TryParse(firstarg, out float vol))
            {
                Plugin.PluginConfig.CassieVolume = vol;
            }
            words.Remove(arguments.At(0));

            Reader.ReadMessage(words, AudioPlayer.TryGet(firstarg, out AudioPlayer player) ? player : Plugin.CassiePlayer);
            response = $"Tried {string.Join(" ", words)}";
            return true;
        }
    }
}
