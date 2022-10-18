using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hub
{
    public class MessageHub: Hub <IMessageHubClient>
    {
        public async Task SendOffersToUser(List <string> message)
        {
            await Clients.All.SendOffersToUsers(message);
        }
    }
    
}
