using System;
using System.Collections.Generic;

namespace GameMain.Scripts.Runtime.Message
{
    public static class MessageSystem
    {
        public static Action<object> OnMessagePushed;
        public static Action<object> OnMessagePoped;
        public static Queue<object> MsgQueue { get; } = new Queue<object>();

        public static void PushMessage(object package)
        {
            MsgQueue.Enqueue(package);
            OnMessagePushed?.Invoke(package);
        }

        public static void NextMessage()
        {
            OnMessagePoped?.Invoke(MsgQueue.Dequeue());
        }
    }
}