using Rage;

using System;

namespace RAGENativeUI;

public readonly ref struct RenderTargetUseGuard
{
    private readonly RenderTarget previousRenderId;

    public RenderTargetUseGuard(RenderTarget newRenderId)
    {
        previousRenderId = RenderTarget.Current;
        RenderTarget.Current = newRenderId;
    }

    public void Dispose()
    {
        RenderTarget.Current = previousRenderId;
    }
}

public readonly struct RenderTarget : IEquatable<RenderTarget>
{
    public uint Id { get; }

    public RenderTarget(uint id) => Id = id;

    public RenderTargetUseGuard Use() => new(this);

    public bool Equals(RenderTarget other) => Id == other.Id;
    public override bool Equals(object obj) => obj is RenderTarget other && Equals(other);
    public override int GetHashCode() => Id.GetHashCode();
    public override string ToString() => $"{nameof(RenderTarget)} {{ {nameof(Id)} = {Id} }}";

    public static bool operator ==(RenderTarget left, RenderTarget right) => left.Equals(right);
    public static bool operator !=(RenderTarget left, RenderTarget right) => !(left == right);

    /// <summary>
    /// Gets the screen render target.
    /// </summary>
    public static RenderTarget Default => new(1); // NOTE: GET_DEFAULT_SCRIPT_RENDERTARGET_RENDER_ID always returns 1, don't need to call it
    /// <summary>
    /// Gets the player's mobile phone screen render target.
    /// </summary>
    public static RenderTarget MobilePhone => new(0); // NOTE: GET_MOBILE_PHONE_RENDER_ID always returns 0, don't need to call it
    /// <summary>
    /// Gets or sets the currently active render target.
    /// </summary>
    public static RenderTarget Current
    {
        get => Default; // TODO: read from memory
        set => N.SetTextRenderId(value.Id);
    }
}
