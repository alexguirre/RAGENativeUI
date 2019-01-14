namespace RAGENativeUI.ImGui
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
    using MouseState = rph1::Rage.MouseState;
    using Graphics = rph1::Rage.Graphics;
#else
    /** REDACTED **/
#endif

    using System.Drawing;

    using Rage;

    internal static partial class Gui
    {
        internal struct State
        {
            public Graphics Graphics;

            public MouseState LastMouseState;
            public MouseState CurrentMouseState;
            public Vector2 MousePosition;
            public bool IsMouseEnabled;
            public bool HasMouseBeenCalled;

            public Container ScreenContainer;
            public Container CurrentContainer;

            public ushort ParentId;
            public ushort ElementId;

            public uint DragId;
            public Vector2 DragPosition;

            public void PushContainer(RectangleF drawArea)
            {
                CurrentContainer = new Container(CurrentContainer, drawArea);
            }

            public void PushContainer(RectangleF drawArea, PointF clipOffset)
            {
                CurrentContainer = new Container(CurrentContainer, drawArea, clipOffset);
            }

            public void PopContainer()
            {
                CurrentContainer = CurrentContainer.Parent;
                if (CurrentContainer == null)
                    CurrentContainer = ScreenContainer;
            }

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

        internal class Container
        {
            public readonly Container Parent;
            public readonly RectangleF DrawArea;
            public readonly PointF? OffsetOverride;

            public Container(Container parent, RectangleF drawArea, PointF? offsetOverride = null)
            {
                Parent = parent;
                DrawArea = drawArea;
                OffsetOverride = offsetOverride;
            }

            public Vector2 ConvertToRootCoords(Vector2 localPosition)
            {
                float x = 0.0f;
                float y = 0.0f;
                if (OffsetOverride.HasValue)
                {
                    x = OffsetOverride.Value.X;
                    y = OffsetOverride.Value.Y;
                }


                Vector2 p = new Vector2(localPosition.X + DrawArea.X + x, localPosition.Y + DrawArea.Y + y);
                if(Parent != null)
                {
                    p = Parent.ConvertToRootCoords(p);
                }

                return p;
            }

            public PointF ConvertToRootCoords(PointF localPosition)
            {
                Vector2 p = ConvertToRootCoords(new Vector2(localPosition.X, localPosition.Y));
                return new PointF(p.X, p.Y);
            }

            public RectangleF ConvertToRootCoords(RectangleF localPosition)
            {
                RectangleF r = localPosition;
                r.Location = ConvertToRootCoords(r.Location);
                return r;
            }

            public RectangleF ClipLocalRectangle(RectangleF localPosition)
            {
                float x = 0.0f;
                float y = 0.0f;
                if (OffsetOverride.HasValue)
                {
                    x = OffsetOverride.Value.X;
                    y = OffsetOverride.Value.Y;
                }

                return RectangleF.Intersect(new RectangleF(-x, -y, DrawArea.Width, DrawArea.Height), localPosition);
            }
        }
    }
}

