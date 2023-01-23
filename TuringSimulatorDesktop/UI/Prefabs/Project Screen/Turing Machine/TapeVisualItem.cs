using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringCore;
using TuringSimulatorDesktop.Input;

namespace TuringSimulatorDesktop.UI.Prefabs
{
    public class TapeVisualItem : IVisualElement, ICanvasInteractable
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

        bool isActive;
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                for (int i = 0; i < Cells.Count; i++)
                {
                    Cells[i].IsActive = value;
                }
            }
        }

        public void SetProjectionMatrix(Matrix projectionMatrix, Matrix inverseProjectionMatrix)
        {
            for (int i = 0; i < Cells.Count; i++)
            {
                Cells[i].SetProjectionMatrix(projectionMatrix, inverseProjectionMatrix);
            }
        }

        ActionGroup Group;

        HorizontalLayoutBox CellLayoutBox;
        
        List<TapeCell> Cells;
        public float CameraMin, CameraMax;
        public Tape SourceTape = null;

        public TapeVisualItem(ActionGroup group)
        {
            Group = group;
            CellLayoutBox = new HorizontalLayoutBox();
            CellLayoutBox.DrawBounded = false;

            Cells = new List<TapeCell>();

            IsActive = true;
        }

        public void SetTapeData(Tape Data)
        {
            SourceTape = Data;
            UpdateLayout();
        }

        public void UpdateLayout()
        {
            float Width = CameraMax - CameraMin;
            int TargetCellCount = (int)MathF.Ceiling(Width / TapeCell.ReferenceWidth) + 1;

            if (Cells.Count < TargetCellCount)
            {
                while (Cells.Count < TargetCellCount)
                {
                    TapeCell NewCell = new TapeCell(Group);
                    NewCell.IsActive = true;
                    Cells.Add(NewCell);
                    CellLayoutBox.AddElement(NewCell);
                }
            }
            else if (Cells.Count > TargetCellCount)
            {
                while (Cells.Count > TargetCellCount)
                {
                    Cells[Cells.Count - 1].Close();
                    CellLayoutBox.RemoveElement(Cells[Cells.Count - 1]);
                    Cells.RemoveAt(Cells.Count - 1);
                }
            }

            if (SourceTape != null)
            {
                for (int i = 0; i < Cells.Count; i++)
                {

                }
            }

            CellLayoutBox.Position = new Vector2(CameraMin - (TapeCell.ReferenceWidth - ((Position.X - CameraMin) % TapeCell.ReferenceWidth)), position.Y);
            CellLayoutBox.UpdateLayout();
        }


        void MoveLayout()
        {
            CellLayoutBox.Position = position;
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                CellLayoutBox.Draw(BoundPort);
            }
        }
    }
}
