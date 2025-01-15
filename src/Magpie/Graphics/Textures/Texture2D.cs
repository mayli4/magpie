using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using static Magpie.Graphics.OpenGLApi;

namespace Magpie.Graphics.Textures;

public sealed unsafe class Texture2D : IDisposable {
    public uint Id { get; private set; }
    public uint Width;
    public uint Height;

    public FilterMode FilterMode { get; set; } = FilterMode.Nearest;

    public Texture2D(string path) {
        Id = OpenGL.GenTexture();
        Bind();

        using(var image = Image.Load<Rgba32>(path)) {
            OpenGL.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);
            image.ProcessPixelRows(accessor =>  {
                //imagesharp doesnt store images in contiguous memory by default so send the image row by row
                for (int y = 0; y < accessor.Height; y++) {
                    fixed (void* data = accessor.GetRowSpan(y)) {
                        OpenGL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, y, (uint) accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                    }
                }
            });
        }
        
        SetParameters();
    }

    private void SetParameters() {
        OpenGL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.ClampToEdge);
        OpenGL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.ClampToEdge);
        OpenGL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.LinearMipmapLinear);
        OpenGL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);
        OpenGL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
        OpenGL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
        OpenGL.GenerateMipmap(TextureTarget.Texture2D);
    }

    public void Bind(TextureUnit textureSlot = TextureUnit.Texture0) {
        OpenGL.ActiveTexture(textureSlot);
        OpenGL.BindTexture(TextureTarget.Texture2D, Id);
    }

    public void Dispose(){
        OpenGL.DeleteTexture(Id);
    }
}