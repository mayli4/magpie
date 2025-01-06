using Silk.NET.Core.Contexts;

namespace Magpie.Windowing;

public class Window : IGLContextSource {
    public IGLContext? GLContext { get; }
}