using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasparCG.Osc.net
{

    public class OscHub : Microsoft.AspNet.SignalR.Hub
    {
        
        public OscHub()
        {
           Logger.logInfo("New Hub created");
            
        }
        
 

        public void Send(string name, dynamic message)
        {
         
            Clients.All.oscMessage(name, message);
        }


        public override Task OnConnected()
        {
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }


    }

    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }
}
