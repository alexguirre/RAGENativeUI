#nullable enable
namespace RAGENativeUI.Internals;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using EasyHook;

using Rage;

using Debug = System.Diagnostics.Debug;

public static unsafe class CustomRenderTargetsHook
{
    [Conditional("DEBUG")]
    private static void Log(string str) => Game.LogTrivialDebug($"[RAGENativeUI::{nameof(CustomRenderTargetsHook)}] {str}");


    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool CRenderPhaseScript2d_RenderOnNamedRenderTargets_Delegate();

    private static LocalHook? hook;
    private static IntPtr originalAddr;
    private static CRenderPhaseScript2d_RenderOnNamedRenderTargets_Delegate? original;
    private static CRenderPhaseScript2d_RenderOnNamedRenderTargets_Delegate? detour;

    public static void Init(/*IntPtr stubAddr*/)
    {
        IntPtr addr = Game.FindPattern("48 83 EC 28 8B 05 ?? ?? ?? ?? 89 44 24 40 85 C0 74 50 48 8B 0D ?? ?? ?? ?? 83 4C 24");
        if (addr == IntPtr.Zero)
        {
            Log($"addr pattern not found");
            return;
        }

        *renderTargetsData = default;

        originalAddr = addr;
        original = Marshal.GetDelegateForFunctionPointer<CRenderPhaseScript2d_RenderOnNamedRenderTargets_Delegate>(addr);
        detour = CRenderPhaseScript2d_RenderOnNamedRenderTargets_Detour;
        hook = LocalHook.Create(addr, detour, null);
        hook.ThreadACL.SetExclusiveACL(null);
    }

    public static void Shutdown()
    {
        if (original == null)
        {
            return;
        }

        hook!.Dispose();
        LocalHook.Release();
    }

    private static unsafe bool CRenderPhaseScript2d_RenderOnNamedRenderTargets_Detour()
    {
        var CScript2D_RenderOnRenderTargets = (void*)(originalAddr + *(int*)(originalAddr + 0x53) + 0x57);
        var CDrawCommands_AddCallback_Int32_Int64 = (delegate* unmanaged[Stdcall]<void*, nint, in int, in nint, void*>)(originalAddr + *(int*)(originalAddr + 0x5A) + 0x5E);

        lock (lockObj)
        {
            if (renderTargetsData->Texture != null)
            {
                CDrawCommands_AddCallback_Int32_Int64(CScript2D_RenderOnRenderTargets, 0, 1, (nint)renderTargetsData);
            }
        }

        return original!();
    }

    private static readonly object lockObj = new object();
    private static unsafe NamedRenderTargetBufferedData* renderTargetsData = (NamedRenderTargetBufferedData*)Marshal.AllocHGlobal(sizeof(NamedRenderTargetBufferedData));

    [StructLayout(LayoutKind.Sequential, Size = 0x18)]
    private unsafe struct NamedRenderTargetBufferedData
    {
        public grcRenderTarget* RenderTarget;
        public grcTexture* Texture;
        public uint Id;
        public int field_14;
    }

    public static RenderTarget SetCurrentRenderTarget(RuntimeTextureDictionary txd, string renderTargetTexture)
    {
        lock (lockObj)
        {
            var texture = txd.GetTextureOrThrow(renderTargetTexture); // FIXME: there are no checks to ensure this is a render target!
            renderTargetsData->Texture = texture;
            renderTargetsData->RenderTarget = (grcRenderTarget*)texture;
            renderTargetsData->Id = 12345678;
            return new(renderTargetsData->Id);
        }
    }
}
