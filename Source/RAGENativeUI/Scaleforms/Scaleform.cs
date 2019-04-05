namespace RAGENativeUI.Scaleforms
{
#if RPH1
    extern alias rph1;
    using Vector2 = rph1::Rage.Vector2;
    using Vector3 = rph1::Rage.Vector3;
    using Rotator = rph1::Rage.Rotator;
#else
    /** REDACTED **/
#endif

    using System;
    using System.Drawing;
    using RAGENativeUI.Text;

    public unsafe class Scaleform
    {
        private int handle;

        public string Name { get; }
        public int Handle { get { return handle; } }
        public bool IsLoaded { get { return N.HasScaleformMovieLoaded(handle); } }

        public Scaleform(string name)
        {
            Throw.IfNull(name, nameof(name));

            Name = name;
            Load();
        }

        public virtual void Load()
        {
            handle = N.RequestScaleformMovie(Name);
        }

        public virtual void LoadAndWait()
        {
            Load();

            int endTime = Environment.TickCount + 5000;
            while (!IsLoaded && endTime > Environment.TickCount)
                RPH.GameFiber.Yield();
        }

        public virtual void Dismiss()
        {
            if (IsLoaded)
            {
                N.SetScaleformMovieAsNoLongerNeeded(ref handle);
            }
        }

        public virtual void CallMethod(string methodName)
        {
            Throw.IfNull(methodName, nameof(methodName));

            if (!IsLoaded)
                LoadAndWait();

            N.CallScaleformMovieMethod(handle, methodName);
        }

        public virtual void CallMethod(string methodName, params object[] arguments)
        {
            Throw.IfNull(methodName, nameof(methodName));

            if (!IsLoaded)
                LoadAndWait();

            N.BeginScaleformMovieMethod(handle, methodName);

            if (arguments != null)
            {
                foreach (object arg in arguments)
                {
                    switch (arg)
                    {
                        case int intValue:
                            N.PushScaleformMovieMethodParameterInt(intValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                            break;
                        case uint uintValue:
                            N.PushScaleformMovieMethodParameterInt(unchecked((int)uintValue)); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                            break;
                        case short shortValue:
                            N.PushScaleformMovieMethodParameterInt(shortValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                            break;
                        case ushort ushortValue:
                            N.PushScaleformMovieMethodParameterInt(ushortValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                            break;
                        case sbyte sbyteValue:
                            N.PushScaleformMovieMethodParameterInt(sbyteValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                            break;
                        case byte byteValue:
                            N.PushScaleformMovieMethodParameterInt(byteValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                            break;
                        case bool boolValue:
                            N.PushScaleformMovieMethodParameterBool(boolValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_BOOL
                            break;
                        case float floatValue:
                            N.PushScaleformMovieMethodParameterFloat(floatValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_FLOAT
                            break;
                        case double doubleValue:
                            N.PushScaleformMovieMethodParameterFloat((float)doubleValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_FLOAT
                            break;
                        case string stringValue:
                            N.PushScaleformMovieMethodParameterString(stringValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_STRING
                            break;
                        case char charValue:
                            N.PushScaleformMovieMethodParameterString(charValue.ToString()); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_STRING
                            break;
                        case Text textValue:
                            N.BeginTextCommandScaleformString(textValue.Label);
                            textValue.PushComponents();
                            N.EndTextCommandScaleformString();
                            break;
                        case null: throw new ArgumentNullException($"Null argument passed to scaleform with handle {handle} and name '{Name}' when calling {methodName}.");
                        default: throw new ArgumentException($"Unsupported argument type {arg.GetType()} passed to scaleform with handle {handle} and name '{Name}' when calling {methodName}.");
                    }
                }
            }

            N.EndScaleformMovieMethod();
        }

        public virtual void Draw() => Draw(Color.White);

        public virtual void Draw(Color color)
        {
            if (!IsLoaded)
                LoadAndWait();

            N.DrawScaleformMovieFullscreen(handle, color.R, color.G, color.B, color.A, 0);
        }

        public virtual void Draw(Vector2 position, Vector2 size) => Draw(position, size, Color.White);

        public virtual void Draw(Vector2 position, Vector2 size, Color color)
        {
            if (!IsLoaded)
                LoadAndWait();

            N.DrawScaleformMovie(handle, position.X, position.Y, size.X, size.Y, color.R, color.G, color.B, color.A, 0);
        }

        public virtual void Draw3D(Vector3 position, Rotator rotation, Vector3 scale)
        {
            if (!IsLoaded)
                LoadAndWait();

            N.DrawScaleformMovie3DNonAdditive(handle, position.X, position.Y, position.Z, rotation.Pitch, rotation.Roll, rotation.Yaw, 2f, 2f, 1f, scale.X, scale.Y, scale.Z, 2);
        }
    }
}

