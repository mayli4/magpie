using static Magpie.Graphics.OpenGLApi;

namespace Magpie.Graphics;

public sealed class Framebuffer : IDisposable {
    public readonly uint Id;

    public Framebuffer() {
        Id = OpenGL.GenFramebuffer();
    }
    
    void IDisposable.Dispose() {
        
    }
}