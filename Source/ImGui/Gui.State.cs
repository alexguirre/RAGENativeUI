namespace RAGENativeUI.ImGui
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    using Rage;
    using Rage.Native;
    using Graphics = Rage.Graphics;

    using RAGENativeUI.Rendering;

    public static partial class Gui
    {
        internal struct State
        {
            public Graphics Graphics;

            public MouseState LastMouseState;
            public MouseState CurrentMouseState;
            public Vector2 MousePosition;
            public bool IsMouseEnabled;
            public bool HasMouseBeenCalled;

            public RectangleF CurrentParentContainer;

            public ushort ParentId;
            public ushort ElementId;

            public uint DragId;
            public Vector2 DragPosition;

            public bool IsDraggingAny() => DragId != 0;
            public bool IsDragging(uint elementId) => DragId == elementId;

            public void Drag(uint elementId)
            {
                DragId = elementId;
                DragPosition = MousePosition;
            }

            public void Drop()
            {
                DragId = 0;
            }

            public Vector2 DragOffset()
            {
                Vector2 offset = new Vector2(MousePosition.X - DragPosition.X, MousePosition.Y - DragPosition.Y);
                DragPosition = MousePosition;
                return offset;
            }

            public void ResetIds()
            {
                ParentId = 0;
                ElementId = 0;
            }

            public uint Id(bool increaseParentId = false, bool increaseElementId = true)
            {
                if (increaseParentId)
                {
                    ParentId++;
                    ElementId = 0;
                }

                if (increaseElementId)
                {
                    ElementId++;
                }

                uint id = unchecked((uint)((ParentId << 16) | (ElementId)));

                return id;
            }
        }
    }
}

