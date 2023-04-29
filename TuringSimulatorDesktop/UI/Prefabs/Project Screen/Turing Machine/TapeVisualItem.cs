using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuringCore;
using TuringCore.Files;
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

        //Apply canvas transformations/offsets to all visual cells
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
                
        List<TapeCell> Cells;
        public float CameraMin, CameraMax;
        public Tape SourceTape = null;

        public TapeVisualItem(ActionGroup group)
        {
            Group = group;

            Cells = new List<TapeCell>();

            IsActive = true;
        }

        //Set core model Tape object this tape will represent
        public void SetSourceTape(Tape Data)
        {
            SourceTape = Data;
            UpdateLayout();
        }

        //Here we calculate which cells of the visual tape we can actually see
        public void UpdateLayout()
        {
            //Calculate how many cells we can see in the current view
            int TargetCellCount = Convert.ToInt32(MathF.Ceiling((CameraMax - CameraMin) / TapeCell.ReferenceTotalWidth)) + 1;

            //Add/Remove appropriate number of cells
            //redo!!!, zoom out nmegative value bug, need to multiply movement by zoom too
            if (Cells.Count < TargetCellCount)
            {
                while (Cells.Count < TargetCellCount)
                {
                    TapeCell NewCell = new TapeCell(this, Group);
                    NewCell.IsActive = true;
                    NewCell.SetProjectionMatrix(ProjectionMatrix, InverseProjectionMatrix);
                    Cells.Add(NewCell);
                }
            }
            else if (Cells.Count > TargetCellCount)
            {
                while (Cells.Count > TargetCellCount)
                {
                    Cells[Cells.Count - 1].Close();
                    Cells.RemoveAt(Cells.Count - 1);
                }
            }

            //Calculate what index on tape the first visible node is
            float Difference = CameraMin - position.X;
            int StartCellIndex;

            if (Difference > 0)
            {
                StartCellIndex = Convert.ToInt32(MathF.Ceiling(Difference / TapeCell.ReferenceTotalWidth)) - 1;
            }
            else
            {
                StartCellIndex = Convert.ToInt32(MathF.Floor(Difference / TapeCell.ReferenceTotalWidth));
            }

            //Position each cell where it should be in the view and assign its index on the tape
            for (int i = 0; i < Cells.Count; i++)
            {
                Cells[i].Position = new Vector2(position.X + StartCellIndex * TapeCell.ReferenceTotalWidth, position.Y);

                Cells[i].Index = StartCellIndex;
                Cells[i].IndexLabel.Text = StartCellIndex.ToString();

                StartCellIndex++;
            }

            //Populate visual cells with data from Tape model object if we have any
            if (SourceTape != null)
            {
                for (int i = 0; i < Cells.Count; i++)
                {
                    if (SourceTape.Data.ContainsKey(Cells[i].Index))
                    {
                        Cells[i].InputOutputLabel.Text = SourceTape[Cells[i].Index];
                    }
                    else
                    {
                        Cells[i].InputOutputLabel.Text = SourceTape.DefinitionAlphabet.EmptyCharacter;
                    }
                        /*
                        if (Cells[i].Index > SourceTape.HighestIndex || Cells[i].Index < SourceTape.LowestIndex)
                        {
                            Cells[i].InputOutputLabel.Text = SourceTape.DefinitionAlphabet.EmptyCharacter;
                        }
                        else
                        {
                            Cells[i].InputOutputLabel.Text = SourceTape[Cells[i].Index];
                        }
                        */
                    }
            }

        }

        //Update underlying core model
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

        //Convert position of an index to world space coordinates
        public Vector2 GetIndexWorldPosition(int Index)
        {
            return Position + new Vector2(TapeCell.ReferenceTotalWidth * Index + TapeCell.ReferenceTotalWidth * 0.5f, Position.Y + TapeCell.ReferenceCellHeight * 0.5f);
        }

        void MoveLayout()
        {
            UpdateLayout();
        }

        //Draw all visual cells
        public void Draw(Viewport? BoundPort = null)
        {
            if (IsActive)
            {
                for (int i = 0; i < Cells.Count; i++)
                {
                    Cells[i].Draw(BoundPort);
                }
            }
        }
    }
}
