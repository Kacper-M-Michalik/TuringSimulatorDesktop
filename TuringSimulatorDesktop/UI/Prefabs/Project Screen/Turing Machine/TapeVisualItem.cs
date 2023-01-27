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
            ProjectionMatrix = projectionMatrix;
            InverseProjectionMatrix = inverseProjectionMatrix;

            for (int i = 0; i < Cells.Count; i++)
            {
                Cells[i].SetProjectionMatrix(projectionMatrix, inverseProjectionMatrix);
            }
        }
        Matrix ProjectionMatrix, InverseProjectionMatrix;

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

        public void SetSourceTape(Tape Data)
        {
            SourceTape = Data;
            UpdateLayout();
        }

        public void UpdateLayout()
        {
            float Width = CameraMax - CameraMin;
            int TargetCellCount = Convert.ToInt32(MathF.Ceiling(Width / TapeCell.ReferenceTotalWidth)) + 1;

            //redo!!!, zoom out nmegative value bug, need to multiply movement by zoom too
            if (Cells.Count < TargetCellCount)
            {
                while (Cells.Count < TargetCellCount)
                {
                    TapeCell NewCell = new TapeCell(this, Group);
                    NewCell.IsActive = true;
                    NewCell.SetProjectionMatrix(ProjectionMatrix, InverseProjectionMatrix);
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

            float TapeLeftPosition;

            if (Position.X - CameraMin > CameraMax - Position.X)
            {
                TapeLeftPosition = CameraMin - (TapeCell.ReferenceTotalWidth - ((Position.X - CameraMin) % TapeCell.ReferenceTotalWidth));
            }
            else
            {
                TapeLeftPosition = CameraMin - ((CameraMax- Position.X) % TapeCell.ReferenceTotalWidth);
            }

            int BaseCellIndex = Convert.ToInt32((TapeLeftPosition - Position.X) / TapeCell.ReferenceTotalWidth);

            int CellIndex = BaseCellIndex;
            for (int i = 0; i < Cells.Count; i++)
            {
                Cells[i].Index = CellIndex;
                Cells[i].IndexLabel.Text = CellIndex.ToString();
                CellIndex++;
            }

            CellIndex = BaseCellIndex;
            if (SourceTape != null)
            {
                for (int i = 0; i < Cells.Count; i++)
                {
                    if (CellIndex > SourceTape.HighestIndex || CellIndex < SourceTape.LowestIndex)
                    {
                        Cells[i].InputOutputLabel.Text = SourceTape.DefinitionAlphabet.EmptyCharacter;
                    }
                    else
                    {
                        Cells[i].InputOutputLabel.Text = SourceTape[CellIndex];
                    }
                    CellIndex++;
                }
            }

            CellLayoutBox.Position = new Vector2(TapeLeftPosition, position.Y);
            CellLayoutBox.UpdateLayout();
        }

        public void UpdateTapeContents(int Index, InputBox Sender)
        {
            if (SourceTape != null)
            {
                SourceTape[Index] = Sender.Text;
            }
            else
            {
                Sender.Text = "";
            }
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
