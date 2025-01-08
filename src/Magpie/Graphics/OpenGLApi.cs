using Silk.NET.Core.Contexts;
using Silk.NET.OpenGL;

namespace Magpie.Graphics;

public sealed class OpenGLApi {
    public static GL OpenGL { get; private set; } = null!;

    public static void InitializeOpenGL(
        IGLContext context) {
        Console.Write("Initializing OpenGL...");
        
        OpenGL = GL.GetApi(context);
        
        Console.WriteLine("\nOpenGL Initialized.");
    }

    public static bool CheckGLErrors(string context = null) {
        var error = OpenGL.GetError();
        
        switch (error) {
            case GLEnum.NoError:
                return false;
            default:
                string message = $"Error: '{error}'. Context: '{context ?? "Not provided"}'.";
                
                Console.WriteLine(message);

                return true;
        }
    }
}