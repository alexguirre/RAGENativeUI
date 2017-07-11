namespace RAGENativeUI.Elements
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Utility;

    public class Scaleform
    {
        private int handle;
        public int Handle { get { return handle; } }

        public bool IsLoaded
        {
            get { return NativeFunction.Natives.HasScaleformMovieLoaded<bool>(handle); }
        }

        public Scaleform(string name)
        {
            handle = NativeFunction.Natives.RequestScaleformMovie<int>(name);
        }

        public void Dismiss()
        {
            NativeFunction.Natives.SetScaleformMovieAsNoLongerNeeded(ref handle);
        }

        public void CallMethod(string methodName)
        {
            NativeFunction.Natives.CallScaleformMovieMethod(handle, methodName);
        }

        public void CallMethod(string methodName, params object[] arguments)
        {
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
                    NativeFunction.Natives.BeginTextCommandScaleformString("STRING");
                    NativeFunction.Natives.x5F68520888E69014(arg.ToString()); // _ADD_TEXT_COMPONENT_SCALEFORM
                    NativeFunction.Natives.EndTextCommandScaleformString();
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
                else if (arg is ScaleformTextureDictionaryArgument)
                {
                    NativeFunction.Natives.xBA7148484BD90365(((ScaleformTextureDictionaryArgument)arg).Name); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_STRING
                }
                else
                {
                    throw new ArgumentException($"Unsupported argument type {arg.GetType()} passed to scaleform with handle {handle}.", nameof(arguments));
                }
            }

            NativeFunction.Natives.xC6796A8FFA375E53(); // _POP_SCALEFORM_MOVIE_FUNCTION_VOID
        }

        public void Draw() => Draw(Color.White);

        public void Draw(Color color)
        {
            NativeFunction.Natives.DrawScaleformMovieFullscreen(handle, color.R, color.G, color.B, color.A, 0);
        }

        public void Draw(GameScreenRectangle rectangle) => Draw(rectangle, Color.White);

        public void Draw(GameScreenRectangle rectangle, Color color)
        {
            NativeFunction.Natives.DrawScaleformMovie(handle, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color.R, color.G, color.B, color.A, 0);
        }

        public void Draw3D(Vector3 position, Rotator rotation, Vector3 scale)
        {
            NativeFunction.Natives.x1CE592FDC749D6F5(handle, position.X, position.Y, position.Z, rotation.Pitch, rotation.Roll, rotation.Yaw, 2f, 2f, 1f, scale.X, scale.Y, scale.Z, 2); // _DRAW_SCALEFORM_MOVIE_3D_NON_ADDITIVE
        }
    }


    public struct ScaleformTextureDictionaryArgument
    {
        public readonly string Name;

        public ScaleformTextureDictionaryArgument(string name)
        {
            Name = name;
        }

        public static implicit operator string(ScaleformTextureDictionaryArgument arg) => arg.Name;
        public static implicit operator ScaleformTextureDictionaryArgument(string name) => new ScaleformTextureDictionaryArgument(name);
    }
}

