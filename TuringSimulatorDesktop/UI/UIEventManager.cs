using System;
using System.Collections.Generic;
using TuringCore;
using TuringCore.Networking;
using TuringServer;

namespace TuringSimulatorDesktop.UI
{
    public delegate void SubscriberDataCallback(object Data);

    public static class UIEventManager
    {
        static Dictionary<int, List<SubscriberDataCallback>> FileUpdateSubscribers = new Dictionary<int, List<SubscriberDataCallback>>();
        static Dictionary<Guid, List<SubscriberDataCallback>> GUIDFileUpdateSubscribers = new Dictionary<Guid, List<SubscriberDataCallback>>();

        //UI Elements subscribe for a response for a specific File/Folder by supplying the File/Folder ID they want to wait for, and a function pointer to the function that should be executed with the response data when the response arrives
        public static void Subscribe(int FileID, SubscriberDataCallback Function)
        {
            if (!FileUpdateSubscribers.ContainsKey(FileID)) FileUpdateSubscribers.Add(FileID, new List<SubscriberDataCallback>());
            FileUpdateSubscribers[FileID].Add(Function);
        }
        public static void Subscribe(Guid FileID, SubscriberDataCallback Function)
        {
            if (!GUIDFileUpdateSubscribers.ContainsKey(FileID)) GUIDFileUpdateSubscribers.Add(FileID, new List<SubscriberDataCallback>());
            GUIDFileUpdateSubscribers[FileID].Add(Function);
        }
        public static void Unsubscribe(int FileID, SubscriberDataCallback Function)
        {            
            if (FileUpdateSubscribers.ContainsKey(FileID) && FileUpdateSubscribers[FileID].Contains(Function)) FileUpdateSubscribers[FileID].Remove(Function);
        }
        public static void Unsubscribe(Guid FileID, SubscriberDataCallback Function)
        {
            if (GUIDFileUpdateSubscribers.ContainsKey(FileID) && GUIDFileUpdateSubscribers[FileID].Contains(Function)) GUIDFileUpdateSubscribers[FileID].Remove(Function);
        }

        //Push folder data responses to subscribers
        public static void PushFolderToListeners(int FolderID, FolderDataMessage Data)
        {
            List<SubscriberDataCallback> Subscribers = FileUpdateSubscribers[FolderID];
            
            for (int i = Subscribers.Count - 1; i > -1; i--)
            {
                Subscribers[i](Data);
            }
        }
        //Push file data responses to subscribers
        public static void PushFileToListeners(Guid FileID, FileDataMessage Data)
        {
            List<SubscriberDataCallback> Subscribers = GUIDFileUpdateSubscribers[FileID];

            for (int i = Subscribers.Count - 1; i > -1; i--)
            {
                Subscribers[i](Data);
            }
        }

        //All Special Events we have so far
        public static bool WindowRequiresNextFrameResize;
        public static bool WindowRequiresNextFrameResizeStep;

        public static bool ClientSuccessConnecting;
        public static EventHandler ClientSuccessConnectingDelegate;
        public static bool ClientFailedConnecting;
        public static EventHandler ClientFailedConnectingDelegate;

        public static EventHandler RecievedProjectDataFromServerDelegate;
    }
}
