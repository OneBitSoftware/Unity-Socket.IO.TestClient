using System.Collections.Generic;
using System;
using UniRx;

/// <summary>
/// Coordinates the actions queue and invokes pending actions
/// </summary>
public class MainThreadDispatcher// : IDispatcher
{

    /// <summary>
    /// This list contains all pending functions to be executed.
    /// All operations must be thread-safe, thus it should stay private.
    /// </summary>
    private List<Tuple<Action<string>, string>> PendingActionsQueue = new List<Tuple<Action<string>, string>>();

    private static MainThreadDispatcher _instance;

    // Typical Unity design for a static instance makes this a quasi-singleton
    public static MainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MainThreadDispatcher(); // Instantiate singleton on first use.
            }

            return _instance;
        }
    }

    /// <summary>
    /// Schedule code for execution in the main-thread. 
    /// </summary>
    /// <param name="fn">The Action to be executed</param>
    public void Enqueue(Action<string> fn, string parameter)
    {
        if (PendingActionsQueue == null) return; // defensive programming

        lock (PendingActionsQueue) // Ensure thread safety
        {
            PendingActionsQueue.Add(new Tuple<Action<string>, string>(fn, parameter));
        }
    }

    /// <summary>
    /// Invoke (execute) all pending actions on the main thread 
    /// </summary>
    public void InvokeAllPending()
    {
        if (PendingActionsQueue == null) return; // defensive programming
        if (PendingActionsQueue.Count < 1) return;

        lock (PendingActionsQueue) // Ensure thread safety
        {
            foreach (var action in PendingActionsQueue)
            {
                action.Item1(action.Item2); // Invoke the action.
            }

            PendingActionsQueue.Clear(); // Clear the pending list.
        }
    }

    /// <summary>
    /// Invoke the oldest action added to the queue. Executed on the main thread 
    /// </summary>
    public void InvokeOne()
    {
        if (PendingActionsQueue == null) return;
        if (PendingActionsQueue.Count < 1) return;

        lock (PendingActionsQueue) // Ensure thread safety
        {
            var action = PendingActionsQueue[0]; // takes the oldest item added
            action.Item1(action.Item2); // Invoke the action.
            PendingActionsQueue.RemoveAt(0);
        }
    }
}
