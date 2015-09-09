using Rage;
using Rage.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAGENativeUI.Elements
{
    public interface IElement
    {
        void Draw();
        void Draw(Size offset);

        bool Enabled { get; set; }
        Point Position { get; set; }
        Color Color { get; set; }
    }
}