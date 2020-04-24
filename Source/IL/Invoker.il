#include "coreassembly.h"

.assembly RAGENativeUI.IL
{
    .ver 1:0:0:0
}

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
}