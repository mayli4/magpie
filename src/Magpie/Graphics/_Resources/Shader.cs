using Silk.NET.OpenGL;

namespace Magpie.Graphics;

public sealed class Shader : IDisposable {
    private GL _gl { get; }

    public uint Id { get; private set; }
    
    public Shader() {
        Id = _gl.CreateProgram();
    }
    
    private uint FromFile(
        ShaderType type,
        string path) {
        var source = File.ReadAllText(path);
        var handle = _gl.CreateShader(type);
        _gl.ShaderSource(handle, source);
        _gl.CompileShader(handle);
        
        return handle;
    }
    
    public void Dispose() {
        throw new NotImplementedException();
    }
}