using Microsoft.AspNet.SignalR.Client;

namespace Mercraft.Multiplayer
{
    public class ConnectionManager
    {
        public void Connect()
        {
            var hubConnection = new HubConnection("http://www.mercraft.com/");
            IHubProxy gameHubProxy = hubConnection.CreateHubProxy("GameHub");
        }
    }
}
