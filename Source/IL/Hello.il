#include "coreassembly.h"

.assembly RAGENativeUI.IL
{
  .ver 1:0:0:0
}

.module RAGENativeUI.IL.dll

.class public abstract auto ansi sealed beforefieldinit RAGENativeUI.IL.Hello
  extends [CORE_ASSEMBLY]System.Object
{
  .method public hidebysig static string World() cil managed aggressiveinlining
  {
    ldstr      "Hello World!"
    ret
  }
}