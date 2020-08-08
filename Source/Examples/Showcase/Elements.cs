namespace RNUIExamples.Showcase
{
    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using Rage;
    using System.Drawing;
    using System.Linq;
    using System;
    using System.Collections.Generic;

    internal sealed class Elements : UIMenu
    {
        private int rectId, textId;
        private readonly List<IElement> elements = new List<IElement>();

        public Elements() : base(Plugin.MenuTitle, "ELEMENTS")
        {
            Plugin.Pool.Add(this);

            CreateMenuItems();

            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();
                
                    foreach (var e in elements)
                    {
                        e.Draw();
                    }
                }
            });
        }

        private void CreateMenuItems()
        {
            var addElement = new UIMenuListScrollerItem<string>("Add Element", "", new[] { "Rect", "Text" });
            addElement.Activated += (m, s) =>
            {
                ElementMenu menu = addElement.Index switch
                {
                    0 => new RectMenu(rectId++),
                    1 => new TextMenu(textId++),
                    _ => throw new NotImplementedException(),
                };
                var bindItem = new UIMenuItem(menu.Name);
                AddItem(bindItem);
                BindMenuToItem(menu, bindItem);

                elements.Add(menu.Element);
            };

            AddItems(addElement);
        }

        private abstract class ElementMenu : UIMenu
        {
            public IElement Element { get; }
            public string Name { get; }

            protected ElementMenu(IElement element, string name) : base(Plugin.MenuTitle, $"ELEMENTS: {name}")
            {
                Plugin.Pool.Add(this);
                Element = element;
                Name = name;
            }
        }

        private class RectMenu : ElementMenu
        {
            private ResRectangle Rect => Element as ResRectangle;

            public RectMenu(int id) : base(new ResRectangle(new Point(1920 / 2, 1080 / 2), new Size(100, 100), Color.FromArgb(100, 255, 0, 0)), $"RECT #{id}")
            {
                var x = new UIMenuNumericScrollerItem<int>("X", "", 0, 1920, 1).WithTextEditing();
                x.Value = Rect.Position.X;
                x.IndexChanged += (s, o, n) =>
                {
                    Rect.Position = new Point(x.Value, Rect.Position.Y);
                };

                var y = new UIMenuNumericScrollerItem<int>("Y", "", 0, 1080, 1).WithTextEditing();
                y.Value = Rect.Position.Y;
                y.IndexChanged += (s, o, n) =>
                {
                    Rect.Position = new Point(Rect.Position.X, y.Value);
                };

                var w = new UIMenuNumericScrollerItem<int>("Width", "", 0, 5000, 1).WithTextEditing();
                w.Value = Rect.Size.Width;
                w.IndexChanged += (s, o, n) =>
                {
                    Rect.Size = new Size(w.Value, Rect.Size.Height);
                };

                var h = new UIMenuNumericScrollerItem<int>("Height", "", 0, 5000, 1).WithTextEditing();
                h.Value = Rect.Size.Height;
                h.IndexChanged += (s, o, n) =>
                {
                    Rect.Size = new Size(Rect.Size.Width, h.Value);
                };

                AddItems(x, y, w, h);
                this.WithFastScrollingOn(x, y, w, h);
            }
        }

        private class TextMenu : ElementMenu
        {
            private ResText Text => Element as ResText;

            public TextMenu(int id) : base(new ResText("Caption", new Point(1920 / 2, 1080 / 2), 0.325f), $"TEXT #{id}")
            {
                var x = new UIMenuNumericScrollerItem<int>("X", "", 0, 1920, 1).WithTextEditing();
                x.Value = Text.Position.X;
                x.IndexChanged += (s, o, n) =>
                {
                    Text.Position = new Point(x.Value, Text.Position.Y);
                };

                var y = new UIMenuNumericScrollerItem<int>("Y", "", 0, 1080, 1).WithTextEditing();
                y.Value = Text.Position.Y;
                y.IndexChanged += (s, o, n) =>
                {
                    Text.Position = new Point(Text.Position.X, y.Value);
                };

                var scale = new UIMenuNumericScrollerItem<float>("Scale", "", 0.0f, 100.0f, 0.01f) { Formatter = v => v.ToString("0.00") }.WithTextEditing();
                scale.Value = Text.Scale;
                scale.IndexChanged += (s, o, n) =>
                {
                    Text.Scale = scale.Value;
                };

                AddItems(x, y, scale);
                this.WithFastScrollingOn(x, y, scale);
            }
        }
    }
}
