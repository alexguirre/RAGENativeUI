#include "coreassembly.h"

.module RAGENativeUI.IL.dll

.class public abstract auto ansi sealed beforefieldinit RAGENativeUI.IL.Invoker
    extends [CORE_ASSEMBLY]System.Object
{
    .method public hidebysig static float32 InvokeRetFloat(native int fnPtr) cil managed aggressiveinlining
    {
        .maxstack 1

        ldarg.0
        calli   unmanaged cdecl float32()
        ret
    }
    
    .method public hidebysig static bool marshal(int8) InvokeRetBool(native int fnPtr, int32 arg1) cil managed aggressiveinlining
    {
        .maxstack 2

        ldarg.1
        ldarg.0
        calli   unmanaged cdecl int8(int32)
        ret
    }
    
    .method public hidebysig static bool marshal(int8) InvokeRetBool(native int fnPtr, void* arg1, void* arg2, string marshal(lpstr) arg3) cil managed aggressiveinlining
    {
        .maxstack 4

        ldarg.1
        ldarg.2
        ldarg.3
        ldarg.0
        calli   unmanaged cdecl int8(void*, void*, string)
        ret
    }
    
    .method public hidebysig static bool marshal(int8) InvokeRetBool(native int fnPtr, int32 arg1, int32 arg2, string marshal(lpstr) arg3) cil managed aggressiveinlining
    {
        .maxstack 4

        ldarg.1
        ldarg.2
        ldarg.3
        ldarg.0
        calli   unmanaged cdecl int8(int32, int32, string)
        ret
    }
    
    .method public hidebysig static void* InvokeRetPointer(native int fnPtr, int32 arg1) cil managed aggressiveinlining
    {
        .maxstack 2

        ldarg.1
        ldarg.0
        calli   unmanaged cdecl void*(int32)
        ret
    }
    
    .method public hidebysig static void* InvokeRetPointer(native int fnPtr, void* arg1, uint32 arg2) cil managed aggressiveinlining
    {
        .maxstack 3

        ldarg.1
        ldarg.2
        ldarg.0
        calli   unmanaged cdecl void*(void*, uint32)
        ret
    }
    
    .method public hidebysig static void* InvokeRetPointer(native int fnPtr, void* arg1, uint64 arg2, uint64 arg3, int32 arg4) cil managed aggressiveinlining
    {
        .maxstack 5

        ldarg.1
        ldarg.2
        ldarg.3
        ldarg.s arg4
        ldarg.0
        calli   unmanaged cdecl void*(void*, uint64, uint64, int32)
        ret
    }
    
    .method public hidebysig static void Invoke(native int fnPtr, int32 arg1) cil managed aggressiveinlining
    {
        .maxstack 2

        ldarg.1
        ldarg.0
        calli   unmanaged cdecl void(int32)
        ret
    }
    
    .method public hidebysig static void Invoke(native int fnPtr, int32 arg1, int32 arg2, bool marshal(int8) arg3) cil managed aggressiveinlining
    {
        .maxstack 4

        ldarg.1
        ldarg.2
        ldarg.3
        ldarg.0
        calli   unmanaged cdecl void(int32, int32, int8)
        ret
    }
    
    .method public hidebysig static void Invoke(native int fnPtr, native int arg1) cil managed aggressiveinlining
    {
        .maxstack 2

        ldarg.1
        ldarg.0
        calli   unmanaged cdecl void(native int)
        ret
    }
    
    .method public hidebysig static void Invoke(native int fnPtr, void* arg1, void* arg2) cil managed aggressiveinlining
    {
        .maxstack 3

        ldarg.1
        ldarg.2
        ldarg.0
        calli   unmanaged cdecl void(void*, void*)
        ret
    }
    
    .method public hidebysig static void Invoke(native int fnPtr, float32 arg1, float32 arg2, float32 arg3, float32 arg4, int32 arg5, int32 arg6, int32 arg7, int32 arg8, bool marshal(int8) arg9) cil managed aggressiveinlining
    {
        .maxstack 10

        ldarg.1
        ldarg.2
        ldarg.3
        ldarg.s arg4
        ldarg.s arg5
        ldarg.s arg6
        ldarg.s arg7
        ldarg.s arg8
        ldarg.s arg9
        ldarg.0
        calli   unmanaged cdecl void(float32, float32, float32, float32, int32, int32, int32, int32, int8)
        ret
    }
    
    .method public hidebysig static void Invoke(native int fnPtr, string marshal(lpstr) arg1, string marshal(lpstr) arg2, float32 arg3, float32 arg4, float32 arg5, float32 arg6, float32 arg7, int32 arg8, int32 arg9, int32 arg10, int32 arg11, bool marshal(int8) arg12) cil managed aggressiveinlining
    {
        .maxstack 13

        ldarg.1
        ldarg.2
        ldarg.3
        ldarg.s arg4
        ldarg.s arg5
        ldarg.s arg6
        ldarg.s arg7
        ldarg.s arg8
        ldarg.s arg9
        ldarg.s arg10
        ldarg.s arg11
        ldarg.s arg12
        ldarg.0
        calli   unmanaged cdecl void(string, string, float32, float32, float32, float32, float32, int32, int32, int32, int32, int8)
        ret
    }
    
    .method public hidebysig static void Invoke(native int fnPtr, void* arg1, void* arg2, uint32 arg3, void* arg4) cil managed aggressiveinlining
    {
        .maxstack 5

        ldarg.1
        ldarg.2
        ldarg.3
        ldarg.s arg4
        ldarg.0
        calli   unmanaged cdecl void(void*, void*, uint32, void*)
        ret
    }
    
    .method public hidebysig static void Invoke(native int fnPtr, uint32 arg1, uint32 arg2, void* arg3) cil managed aggressiveinlining
    {
        .maxstack 4

        ldarg.1
        ldarg.2
        ldarg.3
        ldarg.0
        calli   unmanaged cdecl void(uint32, uint32, void*)
        ret
    }
}