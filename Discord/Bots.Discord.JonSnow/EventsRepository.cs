namespace Bots.Discord.JonSnow
{
    using System;
    using System.Threading.Tasks;
    using Bots.Common.Services;
    using global::Discord.WebSocket;

    public class EventsRepository
    {
        private readonly IJonSnowService jonSnowService;

        public EventsRepository(IJonSnowService jonSnowService)
        {
            this.jonSnowService = jonSnowService;
        }

        public async Task MessageReceived(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message))
            {
                return;
            }

            if (message.Author.IsBot)
            {
                return;
            }

            if (
                message.Content.Contains("jon", StringComparison.InvariantCultureIgnoreCase) || 
                message.Content.Contains("snow", StringComparison.InvariantCultureIgnoreCase) ||
                message.Content.Contains("bastard", StringComparison.InvariantCultureIgnoreCase))
            {
                var isTyping = message.Channel.EnterTypingState();
                var jonSnowSays = await this.jonSnowService.GetResponse(message.Content);
                await message.Channel.SendMessageAsync(jonSnowSays);
                isTyping.Dispose();
            }
        }
    }
}
