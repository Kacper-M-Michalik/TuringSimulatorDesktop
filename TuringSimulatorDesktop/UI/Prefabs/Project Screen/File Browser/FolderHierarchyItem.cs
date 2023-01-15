using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class FolderHierarchyItem : IVisualElement
    {
        Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                MoveLayout();
            }
        }

        Point bounds;
        public Point Bounds
        {
            get => bounds;
            set
            {
            }
        }

        bool isActive = true;
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                SelectFolderButton.IsActive = value;
            }
        }

        ActionGroup Group;

        ColorButton SelectFolderButton;
        Label FolderLabel;

        FileBrowserView Browser;
        FileData Data;
        public const int ReferencePadding = 16;

        public FolderHierarchyItem(FileData data,  FileBrowserView browser, ActionGroup group)
        {
            Group = group;
            Browser = browser;
            Data = data;

            SelectFolderButton = new ColorButton(Group);
            SelectFolderButton.BaseColor = GlobalInterfaceData.Scheme.Background;
            SelectFolderButton.HighlightColor = GlobalInterfaceData.Scheme.DarkInteractableAccent;
            SelectFolderButton.HighlightOnMouseOver = true;
            SelectFolderButton.OnClickedEvent += LoadFolder;

            FolderLabel = new Label();
            FolderLabel.Font = GlobalInterfaceData.StandardBoldFont;
            FolderLabel.FontColor = GlobalInterfaceData.Scheme.FontGrayedOutColor;
            FolderLabel.FontSize = GlobalInterfaceData.Scale(12);
            FolderLabel.Text = Data.Name;

            bounds = GlobalInterfaceData.Scale(new Point(FolderLabel.Bounds.X + ReferencePadding, browser.HierarchyLayout.Bounds.Y));
            ResizeLayout();
            Position = new Vector2(0, 0);
        }

        public void LoadFolder(Button Sender)
        {
            Browser.SwitchOpenedFolder(Data.ID);
        }

        void MoveLayout()
        {
            SelectFolderButton.Position = Position;
            FolderLabel.Position = Position + new Vector2(GlobalInterfaceData.Scale(ReferencePadding/2f), bounds.Y/2f);
        }

        void ResizeLayout()
        {
            SelectFolderButton.Bounds = bounds;

            float FontSize = GlobalInterfaceData.Scale(12);
            FolderLabel.FontSize = FontSize;
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                SelectFolderButton.Draw(BoundPort);
                FolderLabel.Draw(BoundPort);
            }
        }

        public void Close()
        {
            IsActive = false;
            SelectFolderButton.IsMarkedForDeletion = true;
        }
    }
}
