namespace RAGENativeUI.Memory.GFx
{
    using System;
    using System.Runtime.InteropServices;

    using Rage;

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct CScaleformStore
    {
        [FieldOffset(0x0038)] public CPool Pool;

        public CScaleformDef* GetPoolItem(int index)
        {
            return (CScaleformDef*)Pool.Get(unchecked((uint)index));
        }

        private static CScaleformStore* instance;
        public static CScaleformStore* GetInstance()
        {
            if (instance == null)
            {
                IntPtr address = Game.FindPattern("48 8D 0D ?? ?? ?? ?? 8B D3 E8 ?? ?? ?? ?? 84 C0 74 18");
                if(address == IntPtr.Zero)
                {
                    Common.Log($"Incompatible game version, couldn't find {nameof(CScaleformStore)} instance.");
                    return null;
                }

                address = address + *(int*)(address + 3) + 7;
                instance = (CScaleformStore*)address;
            }

            return instance;
        }
    }


    [StructLayout(LayoutKind.Explicit, Size = 72)]
    internal unsafe struct ScaleformData1
    {
        [FieldOffset(0x0000)] public short ScaleformIndex; // in ScaleformData2 array


        private static CArray_ScaleformData1* arrayInstance;
        public static CArray_ScaleformData1* GetArrayInstance()
        {
            if (arrayInstance == null)
            {
                IntPtr address = Game.FindPattern("48 8D 35 ?? ?? ?? ?? 48 8D 3C C0 8B 4C FE B8 E8 ?? ?? ?? ?? 84 C0 74 15");
                if (address == IntPtr.Zero)
                {
                    Common.Log($"Incompatible game version, couldn't find {nameof(CArray_ScaleformData1)} instance.");
                    return null;
                }

                address = address + *(int*)(address + 3) + 7;
                arrayInstance = (CArray_ScaleformData1*)address;
            }

            return arrayInstance;
        }


        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct CArray_ScaleformData1
        {
            private ScaleformData1 start;

            public ScaleformData1* Get(int index)
            {
                fixed (ScaleformData1* array = &start)
                {
                    return &array[index];
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 480)]
    internal unsafe struct ScaleformData2
    {
        [FieldOffset(0x00B0)] public int ScaleformStorePoolIndex;


        private static CSimpleArray_ScaleformData2* arrayInstance;
        public static CSimpleArray_ScaleformData2* GetArrayInstance()
        {
            if (arrayInstance == null)
            {
                IntPtr address = Game.FindPattern("48 8B 0D ?? ?? ?? ?? 48 69 D2 ?? ?? ?? ?? 83 BC 0A ?? ?? ?? ?? ?? 0F 94 C0 C3");
                if (address == IntPtr.Zero)
                {
                    Common.Log($"Incompatible game version, couldn't find {nameof(CSimpleArray_ScaleformData2)} instance.");
                    return null;
                }

                address = address + *(int*)(address + 3) + 7;
                arrayInstance = (CSimpleArray_ScaleformData2*)address;
            }

            return arrayInstance;
        }


        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct CSimpleArray_ScaleformData2
        {
            public ScaleformData2* Offset;
            public short Count;
            public short Size;

            public ScaleformData2* Get(short index)
            {
                if (index >= Size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"The size of this {nameof(CSimpleArray_ScaleformData2)} is {Size}, the index {index} is out of range.");
                }

                return &Offset[index];
            }
        }
    }
}

