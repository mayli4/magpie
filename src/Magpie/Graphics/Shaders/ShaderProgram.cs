using Silk.NET.OpenGL;

namespace Magpie.Graphics.Shaders;

//TODO: Uniforms
//TODO: Sub-shaders? ShaderProgram > VertexShader + FragmentShader + ComputeShader, etc

public sealed class ShaderProgram : IDisposable {
    /// <summary> The "handle" of this shader program that OpenGL will use. </summary>
    public readonly uint Id;

    public readonly string Name;

    public readonly Dictionary<string, ShaderUniform> Uniforms = [];

    public ShaderProgram(
        string vertexPath, 
        string fragmentPath,
        string name = "") {
        
        Id = Magpie.GL.CreateProgram();
        Name = name;
        
        var vertex = FromFile(ShaderType.VertexShader, vertexPath);
        var fragment = FromFile(ShaderType.FragmentShader, fragmentPath);
        
        Magpie.GL.AttachShader(Id, vertex);
        Magpie.GL.AttachShader(Id, fragment);
        Magpie.GL.LinkProgram(Id);
        Magpie.GL.GetProgram(Id, GLEnum.LinkStatus, out int status);
        if(status == 0) {
            throw new Exception($"Program failed to link with error: {Magpie.GL.GetProgramInfoLog(Id)}");
        }
        
        Console.WriteLine($"Program link success! Name: {Name}, Id: {Id}");

        Magpie.GL.DetachShader(Id, vertex);
        Magpie.GL.DetachShader(Id, fragment);
        Magpie.GL.DeleteShader(vertex);
        Magpie.GL.DeleteShader(fragment);
    }

     void IDisposable.Dispose() {
        Magpie.GL.DeleteProgram(Id);
    }

    private uint FromFile(
        ShaderType type, 
        string path) {
        var src = File.ReadAllText(path); // read source from file
        var handle = Magpie.GL.CreateShader(type); // create shader handle
        
        Magpie.GL.ShaderSource(handle, src); // forward the source to OpenGL
        Magpie.GL.CompileShader(handle); // compile source
        
        string infoLog = Magpie.GL.GetShaderInfoLog(handle);
        if(infoLog is null) {
            throw new Exception($"Error compiling shader of type {type}. Failed with error {infoLog}");
        }

        return handle;
    }

    public static void PushShader(
        uint handle) {
        if(handle != 0) {
            Magpie.GL.UseProgram(handle);
        }
        else {
            Magpie.GL.UseProgram(0);
        }
    }
}