using Silk.NET.OpenGL;

namespace Magpie.Graphics.Shaders;

public struct ShaderUniform(string name, UniformType type, int location) {
    public string Name = name;
    public int Location = location;
    public UniformType Type = type;
}