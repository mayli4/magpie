using Silk.NET.OpenGL;
using System.Numerics;
using static Magpie.Graphics.OpenGLApi;

namespace Magpie.Graphics.Shaders;

public sealed class ShaderProgram : IDisposable {
    
    /// <summary> The "handle" of this shader program that OpenGL will use. </summary>
    public readonly uint Id;

    public readonly string Name;

    public readonly Dictionary<string, ShaderUniform> Uniforms = [];

    public ShaderProgram(
        string vertexPath, 
        string fragmentPath,
        string name = "") {
        
        Id = OpenGL.CreateProgram();
        Name = name;
        
        var vertex = FromFile(ShaderType.VertexShader, vertexPath);
        var fragment = FromFile(ShaderType.FragmentShader, fragmentPath);
        
        OpenGL.AttachShader(Id, vertex);
        OpenGL.AttachShader(Id, fragment);
        OpenGL.LinkProgram(Id);
        OpenGL.GetProgram(Id, GLEnum.LinkStatus, out int status);
        
        OpenGL.GetProgram(Id, ProgramPropertyARB.ActiveUniforms, out var uniformCount);

        for(int i = 0; i < uniformCount; i++) {
            OpenGL.GetActiveUniform(Id, (uint)i, 32, out uint length, out int size, out UniformType type, out string uniformName);
            
            var location = OpenGL.GetUniformLocation(Id, uniformName);
            
            Uniforms.Add(uniformName, new ShaderUniform(uniformName, type, location));
            Console.WriteLine($"{Uniforms.Count} uniforms loaded.");
        }
        
        if(status == 0) {
            throw new Exception($"Program failed to link with error: {OpenGL.GetProgramInfoLog(Id)}");
        }
        
        Console.WriteLine($"Program link success! Name: {Name}, Id: {Id}");

        OpenGL.DetachShader(Id, vertex);
        OpenGL.DetachShader(Id, fragment);
        OpenGL.DeleteShader(vertex);
        OpenGL.DeleteShader(fragment);
    }

     void IDisposable.Dispose() {
         OpenGL.DeleteProgram(Id);
    }

    private uint FromFile(
        ShaderType type, 
        string path) {
        var source = File.ReadAllText(path); // read source from file
        var handle = OpenGL.CreateShader(type); // create shader handle
        
        OpenGL.ShaderSource(handle, source); // forward the source to OpenGL
        OpenGL.CompileShader(handle); // compile source
        
        string infoLog = OpenGL.GetShaderInfoLog(handle);
        if(infoLog is null) {
            throw new Exception($"Error compiling shader of type {type}. Failed with error {infoLog}");
        }

        return handle;
    }

    public static void PushShader(
        uint handle) {
        if(handle != 0) {
            OpenGL.UseProgram(handle);
        }
        else {
            OpenGL.UseProgram(0);
        }
    }

    public bool TryGetUniformLocation(string uniformName, out int location) {
        if(Uniforms.TryGetValue(uniformName, out var uniform)) {
            location = uniform.Location;
            
            return true;
        }

        location = -1;
        return false;
    }

    public void SetUniform(string uniformName, float value) {
        var location = OpenGL.GetUniformLocation(Id, uniformName);
        if (location == -1) {
            throw new Exception($"{uniformName} uniform not found in shader.");
        }
        OpenGL.Uniform1(location, value);
    }
    
    public unsafe void SetUniform(string uniformName, Matrix4x4 value) {
        var location = OpenGL.GetUniformLocation(Id, uniformName);
        if (location == -1) {
            throw new Exception($"{uniformName} uniform not found in shader.");
        }
        OpenGL.UniformMatrix4(location, 1, false, (float*) &value);
    }
}