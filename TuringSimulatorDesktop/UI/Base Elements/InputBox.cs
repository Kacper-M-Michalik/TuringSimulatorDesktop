using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI
{
    public delegate void OnEditInputBox(InputBox Sender);
    public delegate void OnClickedInputBox(InputBox Sender);
    public delegate void OnClickedAwayInputBox(InputBox Sender);

    public class InputBox : IVisualElement, ICanvasInteractable, IClickable
    {
        Vector2 position;
        public Vector2 Position
        {
            get => position;
            set
            {
                position = value;
                WorldSpaceMatrix = Matrix.CreateTranslation(new Vector3(Position, 0));
                OutputLabel.Position = position + Labeloffset + new Vector2(0, OutputLabel.Bounds.Y/2);
            }
        }

        Point bounds;
        public Point Bounds
        {
            get => bounds;
            set
            {
                bounds = value;
                BackgroundMesh = Mesh.CreateRectangle(Vector2.Zero, bounds);
                OutputLabel.Bounds = bounds;
            }
        }

        public bool IsActive { get; set; } = true;
        public bool IsMarkedForDeletion { get; set; }

        public void SetProjectionMatrix(Matrix projectionMatrix, Matrix inverseProjectionMatrix)
        {
            ProjectionMatrix = projectionMatrix;
            InverseProjectionMatrix = inverseProjectionMatrix;
            OutputLabel.SetProjectionMatrix(projectionMatrix, inverseProjectionMatrix);
        }

        ActionGroup Group;

        Matrix ProjectionMatrix = Matrix.Identity;
        Matrix InverseProjectionMatrix = Matrix.Identity;
        Matrix WorldSpaceMatrix = Matrix.Identity;
        public Mesh BackgroundMesh = Mesh.CreateRectangle(Vector2.Zero, new Point(10, 10));
        public Color BackgroundColor = GlobalInterfaceData.Scheme.UIOverlayDebugColor1;

        public Label OutputLabel;
        public Vector2 Labeloffset;

        //TODO - NOT FULLY IMPLEMENTED
        public KeyboardModifiers Modifiers = new KeyboardModifiers();
        StringBuilder Builder = new StringBuilder();
        public bool IsFocused;
        public event OnEditInputBox EditEvent;
        public event OnClickedAwayInputBox ClickEvent;
        public event OnClickedAwayInputBox ClickAwayEvent;
        public string Text 
        {
            get => OutputLabel.Text;
            set
            {
                OutputLabel.Text = value;
                Builder.Clear();
                Builder.Append(Text);
            }
        }

        public InputBox(ActionGroup group)
        {
            Group = group;
            group.ClickableObjects.Add(this);
            GlobalInterfaceData.OSWindow.TextInput += TextInput;

            OutputLabel = new Label(0, 0);
            Bounds = new Point(20, 20);
            Position = Vector2.Zero;
        }

        public InputBox(int width, int height, ActionGroup group)
        {
            Group = group;
            group.ClickableObjects.Add(this);
            GlobalInterfaceData.OSWindow.TextInput += TextInput;

            OutputLabel = new Label(0, 0);
            Bounds = new Point(width, height);
            Position = Vector2.Zero;
        }

        public void Clicked()
        {
            IsFocused = true;
            ClickEvent?.Invoke(this);
        }

        public void ClickedAway()
        {                        
            IsFocused = false;
            ClickAwayEvent?.Invoke(this);
        }

        public bool IsMouseOver()
        {
            Vector3 MousePosition = (InputManager.MousePositionMatrix * InverseProjectionMatrix).Translation;
            return (IsActive && MousePosition.X >= Position.X && MousePosition.X <= Position.X + bounds.X && MousePosition.Y >= Position.Y && MousePosition.Y <= Position.Y + bounds.Y);
        }

        public void TextInput(object Sender, TextInputEventArgs Args)
        {
            bool IllegalInput = false;
            if (IsActive && IsFocused)
            {
                switch (Args.Key)
                {
                    case Keys.Tab:
                        Builder.Append("    ");
                        break;
                    case Keys.Enter:
                        if (Modifiers.AllowsNewLine) Builder.Append("/n");
                        else IllegalInput = true;
                        break;
                    case Keys.Back:
                        if (Builder.Length > 0)
                        {
                            if (Builder.Length > 1 && Builder[Builder.Length - 1] == 'n' && Builder[Builder.Length - 2] == '/') Builder.Remove(Builder.Length - 2, 2);
                            else Builder.Remove(Builder.Length - 1, 1);
                        }
                        break;
                    default:                        
                        if ((char.IsNumber(Args.Character) && Modifiers.AllowsNumbers) || (char.IsLetter(Args.Character) && Modifiers.AllowsCharacters) || (char.IsSymbol(Args.Character) && Modifiers.AllowsSymbols)) Builder.Append(Args.Character);
                        break;
                }

                OutputLabel.Text = Builder.ToString();

                if (!IllegalInput) EditEvent?.Invoke(this);            
            }
           
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                if (IsMouseOver()) InputManager.IsMouseOverTypingArea = true;
                GlobalMeshRenderer.Draw(BackgroundMesh, WorldSpaceMatrix * ProjectionMatrix, BackgroundColor, BoundPort);                
                OutputLabel.Draw(BoundPort);
            }
        }

        public void Close()
        {
            Group.IsDirtyClickable = true;
            IsMarkedForDeletion = true;
        }
    }
}
