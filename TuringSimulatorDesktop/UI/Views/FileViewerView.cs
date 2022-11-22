using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringServer;
using TuringCore;
using TuringSimulatorDesktop;

namespace TuringSimulatorDesktop.UI
{
    /*
    public class FileViewerView : View
    {
        List<FileData> Files;

        List<Button> Elements;

        int CurrentlyViewedFolder = 0;

        public void FilesUpdated(Packet Data)
        {
            CustomConsole.Log("CLIENT: Window received Folder Data");

            CurrentlyViewedFolder = Data.ReadInt();

            Files = new List<FileData>();
            int FolderCount = Data.ReadInt();
            for (int i = 0; i < FolderCount; i++)
            {
                Files.Add(new FileData(Data.ReadString(), Data.ReadInt(), FileType.Folder));
            }
            int FileCount = Data.ReadInt();
            for (int i = 0; i < FileCount; i++)
            {
                Files.Add(new FileData(Data.ReadString(), Data.ReadInt(), FileType.File));
            }

            Elements = new List<Button>();

            Vector2 NewPosition = new Vector2(10, 10);
            for (int i = 0; i < Files.Count; i++)
            {
                if (Files[i].Type == FileType.Folder)
                {
                    //Button Button = new Button(NewPosition, GlobalGraphicsData.TextureLookup[TextureLookupKey.StateNodeBackground], Files[i].Name);
                   // Button.ClickEvent += (Button) => { ClickedFileIcon(Files[i].ID); };
                   // Elements.Add(Button);
                }
                else
                {
                    //change icon later
                   // Button Button = new Button(NewPosition, GlobalGraphicsData.TextureLookup[TextureLookupKey.StateNodeBackground], Files[i].Name);
                   // Button.ClickEvent += (Button) => { ClickedFileIcon(Files[i].ID); };
                   // Elements.Add(Button);
                }

                NewPosition = new Vector2(NewPosition.X + 30, NewPosition.Y);
            }
        }

        public void ClickedFileIcon(int ID)
        {
            if (!UIEventManager.FileUpdateSubscribers.ContainsKey(ID))
            {
                UIEventManager.FileUpdateSubscribers.Add(ID, new List<SubscriberDataCallback>());
            }
            
            UIEventManager.FileUpdateSubscribers[ID].Add(FilesUpdated);
            ClientSendFunctions.RequestFolderData(ID);
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }

        /*
        public override void Draw(SpriteBatch OwnerSpriteBatch, RenderTarget2D PreviousRenderTarget)
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                Elements[i].Draw(OwnerSpriteBatch, PreviousRenderTarget);
            }
        }
        */
    

   // }

    public enum FileType {File, Folder};

    public class FileData
    {
        public string Name;
        public int ID;
        public FileType Type;
    
        public FileData(string SetName, int SetID, FileType SetType)
        {
            Name = SetName;
            ID = SetID;
            Type = SetType;
        }
    }
}

