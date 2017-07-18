namespace RAGENativeUI.Elements
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Utility;

    public class Scaleform
    {
        public string Name { get; }

        private int handle;
        public int Handle { get { return handle; } }

        public bool IsLoaded { get { return NativeFunction.Natives.HasScaleformMovieLoaded<bool>(handle); } }

        public Scaleform(string name)
        {
            Name = name;
            Load();
        }

        public virtual void Load()
        {
            handle = NativeFunction.Natives.RequestScaleformMovie<int>(Name);
        }

        public virtual void LoadAndWait()
        {
            Load();

            int endTime = Environment.TickCount + 5000;
            while (!IsLoaded && endTime > Environment.TickCount)
                GameFiber.Yield();
        }

        public virtual void Dismiss()
        {
            if (IsLoaded)
            {
                NativeFunction.Natives.SetScaleformMovieAsNoLongerNeeded(ref handle);
            }
        }

        public virtual void CallMethod(string methodName)
        {
            if (!IsLoaded)
                LoadAndWait();

            NativeFunction.Natives.CallScaleformMovieMethod(handle, methodName);
        }

        public virtual void CallMethod(string methodName, params object[] arguments)
        {
            if (!IsLoaded)
                LoadAndWait();

            NativeFunction.Natives.xF6E48914C7A8694E(handle, methodName); // _PUSH_SCALEFORM_MOVIE_FUNCTION

            foreach (object arg in arguments)
            {
                if (arg is int)
                {
                    NativeFunction.Natives.xC3D0841A0CC546A6((int)arg); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                }
                else if (arg is uint)
                {
                    NativeFunction.Natives.xC3D0841A0CC546A6((uint)arg); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                }
                else if (arg is string || arg is char)
                {
                    NativeFunction.Natives.xBA7148484BD90365(arg.ToString()); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_STRING
                }
                else if (arg is float)
                {
                    NativeFunction.Natives.xD69736AAE04DB51A((float)arg); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_FLOAT
                }
                else if (arg is double)
                {
                    NativeFunction.Natives.xD69736AAE04DB51A((float)(double)arg); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_FLOAT
                }
                else if (arg is bool)
                {
                    NativeFunction.Natives.xC58424BA936EB458((bool)arg); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_BOOL
                }
                else if (arg is ushort)
                {
                    NativeFunction.Natives.xC3D0841A0CC546A6((uint)(ushort)arg); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                }
                else if (arg is short)
                {
                    NativeFunction.Natives.xC3D0841A0CC546A6((int)(short)arg); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                }
                else if (arg is byte)
                {
                    NativeFunction.Natives.xC3D0841A0CC546A6((uint)(byte)arg); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                }
                else if (arg is sbyte)
                {
                    NativeFunction.Natives.xC3D0841A0CC546A6((int)(sbyte)arg); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                }
                else
                {
                    throw new ArgumentException($"Unsupported argument type {arg.GetType()} passed to scaleform with handle {handle} and name '{Name}'.", nameof(arguments));
                }
            }

            NativeFunction.Natives.xC6796A8FFA375E53(); // _POP_SCALEFORM_MOVIE_FUNCTION_VOID
        }

        public virtual void Draw() => Draw(Color.White);

        public virtual void Draw(Color color)
        {
            if (!IsLoaded)
                LoadAndWait();

            NativeFunction.Natives.DrawScaleformMovieFullscreen(handle, color.R, color.G, color.B, color.A, 0);
        }

        public virtual void Draw(GameScreenRectangle rectangle) => Draw(rectangle, Color.White);

        public virtual void Draw(GameScreenRectangle rectangle, Color color)
        {
            if (!IsLoaded)
                LoadAndWait();

            NativeFunction.Natives.DrawScaleformMovie(handle, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color.R, color.G, color.B, color.A, 0);
        }

        public virtual void Draw3D(Vector3 position, Rotator rotation, Vector3 scale)
        {
            if (!IsLoaded)
                LoadAndWait();

            NativeFunction.Natives.x1CE592FDC749D6F5(handle, position.X, position.Y, position.Z, rotation.Pitch, rotation.Roll, rotation.Yaw, 2f, 2f, 1f, scale.X, scale.Y, scale.Z, 2); // _DRAW_SCALEFORM_MOVIE_3D_NON_ADDITIVE
        }
    }
}

