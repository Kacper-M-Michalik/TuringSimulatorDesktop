using System;
using System.Collections.Generic;
using TuringCore;
using TuringServer;
using TuringSimulatorDesktop.UI;

namespace TuringSimulatorDesktop
{
    public delegate void SubscriberDataCallback(Packet Data);

    public static class UIEventManager
    {
        static Dictionary<int, List<SubscriberDataCallback>> FileUpdateSubscribers = new Dictionary<int, List<SubscriberDataCallback>>();

        public static void Subscribe(int FileID, SubscriberDataCallback Function)
        {
            if (!FileUpdateSubscribers.ContainsKey(FileID)) FileUpdateSubscribers.Add(FileID, new List<SubscriberDataCallback>());
            FileUpdateSubscribers[FileID].Add(Function);
        }
        public static void Unsubscribe(int FileID, SubscriberDataCallback Function)
        {            
            if (FileUpdateSubscribers.ContainsKey(FileID) && FileUpdateSubscribers[FileID].Contains(Function)) FileUpdateSubscribers[FileID].Remove(Function);
        }

        public static void PushToListeners(int FileID, Packet Data)
        {
            //if (!FileUpdateSubscribers.ContainsKey(FileID)) return;

            List<SubscriberDataCallback> Subscribers = FileUpdateSubscribers[FileID];
            int BasePointer = Data.ReadPointerPosition;      
            
            for (int i = Subscribers.Count - 1; i > -1; i--)
            {
                Subscribers[i](Data);
                Data.ReadPointerPosition = BasePointer;
            }
        }

        public static bool WindowRequiresNextFrameResize;
        public static bool WindowRequiresNextFrameResizeStep;

        public static bool ClientSuccessConnecting;
        public static EventHandler ClientSuccessConnectingDelegate;
        public static bool ClientFailedConnecting;
        public static EventHandler ClientFailedConnectingDelegate;

        public static EventHandler RecievedProjectDataFromServerDelegate;

        //public static EventHandler RecievedErrorNotification;
        // public static EventHandler UpdateFileBrowser;

    }
}
