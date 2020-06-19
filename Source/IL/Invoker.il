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
    
    .method public hidebysig static void* InvokeRetPointer(native int fnPtr, void* arg1, uint32 arg1) cil managed aggressiveinlining
    {
        .maxstack 3

        ldarg.1
        ldarg.2
        ldarg.0
        calli   unmanaged cdecl void*(void*, uint32)
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
}