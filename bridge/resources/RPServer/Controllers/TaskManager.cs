using System;
using System.Threading;
using System.Threading.Tasks;
using GTANetworkAPI;
using RPServer.Util;

namespace RPServer.Controllers
{
    internal static class TaskManager
    {
        /// <summary>
        /// THIS NEEDS SOME FORM OF CONTROL TO PREVENT SPAMMING OF TASKS FROM CLIENTS
        /// CAUSING OUR QUEUE TO GET REALLY BIG
        /// </summary>
        public static void Run(Client client, Action action, bool force = false)
        {
            if (client == null) throw new Exception("Invoked TaskManager without refrencing a client instance.");

            if (force)
            {
                Task.Run(action).ContinueWith(HandleTaskCompletion);
                return;
            }

            // Enqueue the Task
            client.GetActionQueue().Enqueue(action);

        }

        private static void HandleTaskCompletion(Task t)
        {
            if (t.IsFaulted && t.Exception != null) throw t.Exception;
        }

        public static void OnHandleDequeue(object state)
        {
            var client = (Client) state;

            // We stop the timer too here and handle it in Run(..) too
            if(client == null) return;

            // Reset the timer
            client.GetActionQueueTimer().Change(200, Timeout.Infinite);
            
            var hasNext = client.GetActionQueue().TryDequeue(out var action);
            if (!hasNext)
                return;

            // If they are currently running another task
            if (!client.IsRunningTask())
                return;

            client.SetRunningTaskState(false);
            Task.Run(action).ContinueWith(HandleTaskCompletion).ContinueWith(task => client.SetRunningTaskState(true));
        }
    }
}