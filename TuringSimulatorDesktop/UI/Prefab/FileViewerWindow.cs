using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringBackend;
using TuringBackend.Logging;
using TuringSimulatorDesktop.Main;

namespace TuringSimulatorDesktop.UI
{
    public class FileViewerWindow : UIElement
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

            for (int i = 0; i < Files.Count; i++)
            {
                if (Files[i].Type == FileType.Folder)
                {
                    Button Button = new Button(UIElement.TextureLookup[TextureLookupKey.StateNodeBackground], Vector2.Zero, Files[i].Name);
                    Button.Clicked += (Button) => { ClickedFileIcon(Files[i].ID); };
                    Elements.Add(Button);
                }
                else
                {
                    //change icon later
                    Button Button = new Button(UIElement.TextureLookup[TextureLookupKey.StateNodeBackground], Vector2.Zero, Files[i].Name);
                    Button.Clicked += (Button) => { ClickedFileIcon(Files[i].ID); };
                    Elements.Add(Button);
                }
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

        public override void Draw(SpriteBatch OwnerSpriteBatch, RenderTarget2D PreviousRenderTarget)
        {

        }

    }

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

