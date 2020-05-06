#include "coreassembly.h"

.module RAGENativeUI.IL.dll

// based on https://github.com/dotnet/runtime/blob/master/src/libraries/System.Runtime.CompilerServices.Unsafe/src/System.Runtime.CompilerServices.Unsafe.il
.class public abstract auto ansi sealed beforefieldinit RAGENativeUI.IL.Unsafe
    extends [CORE_ASSEMBLY]System.Object
{
    .method public hidebysig static !!T& AsRef<T>(native int source) cil managed aggressiveinlining
    {
        .locals (int32&)
        .maxstack 1
        ldarg.0
        // Roundtrip via a local to avoid type mismatch on return that the JIT inliner chokes on.
        stloc.0
        ldloc.0
        ret
    }

    .method public hidebysig static !!T& AsRef<T>(void* source) cil managed aggressiveinlining
    {
        .locals (int32&)
        .maxstack 1
        ldarg.0
        // Roundtrip via a local to avoid type mismatch on return that the JIT inliner chokes on.
        stloc.0
        ldloc.0
        ret
    }
    
    .method public hidebysig static void* AsPointer<T>(!!T& 'value') cil managed aggressiveinlining
    {
        .maxstack 1
        ldarg.0
        conv.u
        ret
    }
}