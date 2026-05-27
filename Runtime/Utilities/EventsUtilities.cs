using System;
using System.Text;
using UnityEngine;

public static class EventsUtilities
{
    public static void DisplaySubscribers(this Delegate action)
    {
        var delegates = action.GetInvocationList();
        StringBuilder message = new StringBuilder();
        message.Append($"Methods: \n");

        for (int i = 0; i < delegates.Length; i++)
        {
            message.Append($"{delegates[i].Method.Name}. ");
        }

        Debug.Log(message.ToString());
    }
}
