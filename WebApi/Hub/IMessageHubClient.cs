namespace WebApi.Hub
{
    public interface IMessageHubClient
    {
        Task SendOffersToUsers(List<string> message);
    }
}
