#if EXILED
namespace CassieReplacement.Commands
{
    using CommandSystem;
    using CommandSystem.Commands.RemoteAdmin;
    using Exiled.API.Features;
    using Respawning;
    using System;
    using Utils;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class CassieCommand : ICommand, IUsageProvider
    {
        public string Command { get; } = "cassie";

        public string[] Aliases { get; }

        public string Description { get; } = "Sends an announcement over the CASSIE system.";

        public string[] Usage { get; } = new string[1] { "message" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "You need the CASSIE remote controller item.";
            Player player = Player.Get(sender);
            if (player is not null)
            {
                if (!CassieControllerItem.Singleton.Check(player.CurrentItem))
                {
                    return false;
                }
            }

            if (arguments.Count < 1)
            {
                response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
                return false;
            }

            string text = RAUtils.FormatArguments(arguments, 0);
            ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " started a cassie announcement: " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging);
            RespawnEffectsController.PlayCassieAnnouncement(text, makeHold: false, makeNoise: true, customAnnouncement: true);
            response = "Announcement sent!";
            return true;
        }
    }
}
#endif