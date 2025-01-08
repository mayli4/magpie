using Magpie.Common;
using Magpie.Graphics;
using Magpie.Graphics.Shaders;
using Silk.NET.OpenGL;
using Silk.NET.GLFW;
using System.Numerics;
using static Magpie.Graphics.OpenGLApi;

namespace Magpie;

// ReSharper disable InconsistentNaming
class Program {
    static void Main() {
        var magpie = new Magpie();
        magpie.Initialize();
    }
}

#nullable disable
public unsafe class Magpie {
    private const int _screen_height = 800;
    private const int _screen_width = 800;
    private const string _window_title = "magpie";

    public static GL GL;

    public int BackingField {
        get => 1;
        
    }

    public Glfw GLFW;
    
    private GlfwContext _glfwContext;
    private GlfwContext _glContext => _glfwContext;
    private WindowHandle* _windowHandle { get; set; }

    private static uint _vertexBuffer;
    private static uint _vertexArray;

    private static ShaderProgram _basicShader;
    
    private static readonly float[] _vertices =
    [
        -1.0f, -1.0f, 0.0f,
        1.0f, -1.0f, 0.0f,
        0.0f,  1.0f, 0.0f
    ];
    
    public void Initialize() {
        GLFW = Glfw.GetApi();
        
        if(!GLFW.Init()) {
            throw new Exception("GLFW Initialization failed.");
        }
        Console.WriteLine("GLFW Initialization success.");
        
        GLFW.WindowHint(WindowHintInt.Samples, 4);
        GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 3);
        GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 2);
        GLFW.WindowHint(WindowHintBool.OpenGLForwardCompat, true);
        GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);

        _windowHandle = GLFW.CreateWindow(_screen_width, _screen_height, _window_title, null, null);

        _glfwContext = new(GLFW, _windowHandle);
        GLFW.MakeContextCurrent(_windowHandle);
        OpenGLApi.InitializeOpenGL(_glContext);
        GL = GL.GetApi(_glContext);

        _basicShader = new ShaderProgram("Resources/Shaders/baseVertex.vert", "Resources/Shaders/baseFragment.frag", "BaseShader");
        
        _vertexArray = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArray);
        _vertexBuffer = GL.GenBuffer();
        GL.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBuffer);
        fixed (void* v = &_vertices[0]) {
            GL.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (_vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw); //Setting buffer data.
        }

        //tell opengl how to give the data to shaders
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
        GL.EnableVertexAttribArray(0);
        
        while(!GLFW.WindowShouldClose(_windowHandle)) {
            GL.Clear((uint) ClearBufferMask.ColorBufferBit);

            GL.BindVertexArray(_vertexArray);
            ShaderProgram.PushShader(_basicShader.Id);
            
            _basicShader.SetUniform("modelViewProjection", mat);

            GL.DrawArrays(PrimitiveType.Triangles, 0, (uint)_vertices.Length);
            
            GLFW.SwapBuffers(_windowHandle);
            GLFW.PollEvents();
        }
        
        GLFW.Terminate();
    }

    public void Update() {
        GLFW.PollEvents();
    }
}