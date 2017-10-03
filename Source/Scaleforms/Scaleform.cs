namespace RAGENativeUI.Scaleforms
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Memory;
    using RAGENativeUI.Memory.GFx;

    public unsafe class Scaleform : IAddressable
    {
        private int handle;

        public string Name { get; }
        public int Handle { get { return handle; } }
        public bool IsLoaded { get { return NativeFunction.Natives.HasScaleformMovieLoaded<bool>(handle); } }
        /// <summary>
        /// Gets the memory address of this instance. If this <see cref="Scaleform"/> isn't loaded, returns <see cref="IntPtr.Zero"/>.
        /// </summary>
        public IntPtr MemoryAddress { get { return IsLoaded ? (IntPtr)GetMovieRoot() : IntPtr.Zero; } }

        public Color BoundingBoxColor
        {
            get
            {
                if (!IsLoaded)
                    return Color.Empty;
                GFxMovieRoot* movie = GetMovieRoot();

                return movie != null ? Color.FromArgb(movie->BackgroundColorAlpha, movie->BackgroundColorRed, movie->BackgroundColorGreen, movie->BackgroundColorBlue) : Color.Empty;
            }
            set
            {
                if (!IsLoaded)
                    return;
                GFxMovieRoot* movie = GetMovieRoot();

                if (movie == null)
                    return;

                movie->BackgroundColorAlpha = value.A;
                movie->BackgroundColorRed = value.R;
                movie->BackgroundColorGreen = value.G;
                movie->BackgroundColorBlue = value.B;
            }
        }

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
                switch (arg)
                {
                    case int intValue:
                        NativeFunction.Natives.xC3D0841A0CC546A6(intValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                        break;
                    case uint uintValue:
                        NativeFunction.Natives.xC3D0841A0CC546A6(uintValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                        break;
                    case short shortValue:
                        NativeFunction.Natives.xC3D0841A0CC546A6(shortValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                        break;
                    case ushort ushortValue:
                        NativeFunction.Natives.xC3D0841A0CC546A6(ushortValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                        break;
                    case sbyte sbyteValue:
                        NativeFunction.Natives.xC3D0841A0CC546A6(sbyteValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                        break;
                    case byte byteValue:
                        NativeFunction.Natives.xC3D0841A0CC546A6(byteValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_INT
                        break;
                    case bool boolValue:
                        NativeFunction.Natives.xC58424BA936EB458(boolValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_BOOL
                        break;
                    case float floatValue:
                        NativeFunction.Natives.xD69736AAE04DB51A(floatValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_FLOAT
                        break;
                    case double doubleValue:
                        NativeFunction.Natives.xD69736AAE04DB51A((float)doubleValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_FLOAT
                        break;
                    case string stringValue:
                        NativeFunction.Natives.xBA7148484BD90365(stringValue); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_STRING
                        break;
                    case char charValue:
                        NativeFunction.Natives.xBA7148484BD90365(charValue.ToString()); // _PUSH_SCALEFORM_MOVIE_FUNCTION_PARAMETER_STRING
                        break;

                    default: throw new ArgumentException($"Unsupported argument type {arg.GetType()} passed to scaleform with handle {handle} and name '{Name}' when calling {methodName}.", nameof(arguments));
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

        public virtual void Draw(ScreenRectangle rectangle) => Draw(rectangle, Color.White);

        public virtual void Draw(ScreenRectangle rectangle, Color color)
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


        internal GFxMovieRoot* GetMovieRoot()
        {
            int index = Handle - 1;
            short data2Index = GameMemory.ScaleformData1Array->Get(index)->ScaleformIndex;
            int storeIndex = GameMemory.ScaleformData2Array->Get(data2Index)->ScaleformStorePoolIndex;
            GFxMovieRoot* movieRoot = GameMemory.ScaleformStore->GetPoolItem(storeIndex)->MovieObject->GetMovieRoot();
            return movieRoot;
        }
    }
}

