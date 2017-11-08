namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    [StructLayout(LayoutKind.Sequential, Size = 8)]
    internal unsafe struct Pointer<T> where T : struct
    {
        public void* RawPointer;

        public ref T Ref => ref Unsafe.AsRef<T>(RawPointer);
        public bool IsNull => RawPointer == null;

        public Pointer(void* rawPointer)
        {
            RawPointer = rawPointer;
        }

        public Pointer(IntPtr rawPointer)
        {
            RawPointer = rawPointer.ToPointer();
        }

        public override bool Equals(object obj)
        {
            if (obj is Pointer<T>)
            {
                return (RawPointer == ((Pointer<T>)obj).RawPointer);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return unchecked((int)((long)RawPointer));
        }

        public override string ToString()
        {
            return $"0x{((long)RawPointer):X16}";
        }


        public static implicit operator void*(Pointer<T> pointer) => pointer.RawPointer;
        public static implicit operator IntPtr(Pointer<T> pointer) => (IntPtr)pointer.RawPointer;

        public static implicit operator Pointer<T>(void* pointer) => new Pointer<T>(pointer);
        public static implicit operator Pointer<T>(IntPtr pointer) => new Pointer<T>(pointer);

        public static bool operator ==(Pointer<T> left, Pointer<T> right) => left.RawPointer == right.RawPointer;
        public static bool operator !=(Pointer<T> left, Pointer<T> right) => left.RawPointer != right.RawPointer;

        public static bool operator ==(IntPtr left, Pointer<T> right) => left.ToPointer() == right.RawPointer;
        public static bool operator !=(IntPtr left, Pointer<T> right) => left.ToPointer() != right.RawPointer;

        public static bool operator ==(Pointer<T> left, IntPtr right) => left.RawPointer == right.ToPointer();
        public static bool operator !=(Pointer<T> left, IntPtr right) => left.RawPointer != right.ToPointer();

        public static bool operator ==(void* left, Pointer<T> right) => left == right.RawPointer;
        public static bool operator !=(void* left, Pointer<T> right) => left != right.RawPointer;

        public static bool operator ==(Pointer<T> left, void* right) => left.RawPointer == right;
        public static bool operator !=(Pointer<T> left, void* right) => left.RawPointer != right;
    }
}

