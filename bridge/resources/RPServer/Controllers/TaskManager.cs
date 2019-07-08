using System;
using System.Threading.Tasks;
using GTANetworkAPI;
using RPServer.Util;

namespace RPServer.Controllers
{
    internal static class TaskManager
    {
        public static void Run(Client client, Action action)
        {
            if(!client.CanRunTask()) throw new Exception($"Mulitple tasks invoked for client ID: {client.Handle}");
            client.SetCanRunTask(false);
            Task.Run(action).ContinueWith(HandleTaskCompletion).ContinueWith(task => client.SetCanRunTask(true));
        }

        private static void HandleTaskCompletion(Task t)
        {
            if (t.IsFaulted && t.Exception != null) throw t.Exception;
        }
    }
}