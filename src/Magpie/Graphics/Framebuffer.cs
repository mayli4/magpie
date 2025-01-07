namespace Magpie.Graphics;

public sealed class Framebuffer : IDisposable {
    public readonly uint Id;

    public Framebuffer() {
        Id = Magpie.GL.GenFramebuffer();
    }
    
    void IDisposable.Dispose() {
        
    }
}