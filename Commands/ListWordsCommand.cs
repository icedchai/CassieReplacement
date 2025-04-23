namespace CassieReplacement.Commands
{
    using CommandSystem;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The command used to invoke <see cref="Reader.ReadMessage(List{string})(System.Collections.Generic.List{string})"/> in-game.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ListWordsCommand : ICommand
    {
        /// <inheritdoc/>
        public string Command => "listwords";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "listwords" };

        /// <inheritdoc/>
        public string Description => "Lists all registered CUSTOMCASSIE words.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string words = "The available words are:\n";
            foreach (string word in Plugin.RegisteredClipNames)
            {
                words += $"{word}, ";
            }

            response = words;
            return true;
        }
    }
}
