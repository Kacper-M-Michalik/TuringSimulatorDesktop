using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                bounds = value;
                ResizeLayout();
            }
        }

        bool isActive;
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
            }
        }

        public void SetProjectionMatrix(Matrix projectionMatrix, Matrix inverseProjectionMatrix)
        {
            for (int i = 0; i < Cells.Count; i++)
            {
                Cells[i].SetProjectionMatrix(projectionMatrix, inverseProjectionMatrix);
            }
        }

        HorizontalLayoutBox TapeContents;
        List<TapeCell> Cells;
        public float CameraMin, CameraMax;

        public TapeVisualItem()
        {
            IsActive = true;

            TapeContents = new HorizontalLayoutBox();
            TapeContents.DrawBounded = false;
            Cells = new List<TapeCell>();
        }

        public void UpdateLayout()
        {
            float Width = CameraMax - CameraMin;
            int TargetCellCount = (int)MathF.Ceiling(Width / TapeCell.ReferenceWidth) + 1;

            if (Cells.Count < TargetCellCount)
            {
                while (Cells.Count < TargetCellCount)
                {
                    TapeCell NewCell = new TapeCell(TapeContents.Group);
                    NewCell.IsActive = true;
                    Cells.Add(NewCell);
                    TapeContents.AddElement(NewCell);
                }
            }
            else if (Cells.Count > TargetCellCount)
            {
                while (Cells.Count > TargetCellCount)
                {
                    TapeContents.RemoveElement(Cells[Cells.Count - 1]);
                    Cells.RemoveAt(Cells.Count - 1);
                }
            }

            for (int i = 0; i < Cells.Count; i++)
            {

            }

            TapeContents.Position = new Vector2(CameraMin - (TapeCell.ReferenceWidth - ((Position.X - CameraMin) % TapeCell.ReferenceWidth)), position.Y);

            TapeContents.Bounds = new Point((Cells.Count) * TapeCell.ReferenceWidth, TapeCell.ReferenceHeight);

            TapeContents.UpdateLayout();

        }


        void MoveLayout()
        {
            TapeContents.Position = position;
        }

        void ResizeLayout()
        {
        }

        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                TapeContents.Draw(BoundPort);
            }
        }
    }
}
