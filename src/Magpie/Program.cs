using Magpie.Graphics;
using Magpie.Graphics.Buffers;
using Magpie.Graphics.Shaders;
using Magpie.Graphics.Textures;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using System.Numerics;
using static Magpie.Graphics.OpenGLApi;

namespace Magpie;

//ungodclass this at some point

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

    public Glfw GLFW;
    
    private GlfwContext _glfwContext;
    private GlfwContext _glContext => _glfwContext;
    private WindowHandle* _windowHandle { get; set; }

    private static uint _vertexBuffer;
    private static uint _colorBuffer;
    private static uint _vertexArray;

    private static ShaderProgram _basicShader;

    private static readonly float[] _colorData = [
        0.583f,  0.771f,  0.014f,
        0.609f,  0.115f,  0.436f,
        0.327f,  0.483f,  0.844f,
        0.822f,  0.569f,  0.201f,
    ];
    
    private static readonly float[] _triangleVerts =
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
        
        OpenGL.Enable(EnableCap.DepthTest);
        OpenGL.Enable(EnableCap.CullFace);
        OpenGL.DepthFunc(DepthFunction.Less);
        
        _basicShader = new ShaderProgram("Resources/Shaders/baseVertex.vert", "Resources/Shaders/baseFragment.frag", "BaseShader");
        
        _vertexArray = OpenGL.GenVertexArray();
        OpenGL.BindVertexArray(_vertexArray);
        
        OpenGL.GenBuffers(1, out _vertexBuffer);
        OpenGL.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBuffer);
        fixed (void* v = &_triangleVerts[0]) {
            OpenGL.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (_triangleVerts.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw);
        }

        OpenGL.GenBuffers(1, out _colorBuffer);
        OpenGL.BindBuffer(BufferTargetARB.ArrayBuffer, _colorBuffer);
        fixed (void* c = &_colorData[0]) {
            OpenGL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(_colorData.Length * sizeof(float)), c, BufferUsageARB.StaticDraw);
        }

        if(GLFW.GetKey(_windowHandle, Keys.Escape) == (int)InputAction.Press) {
            GLFW.Terminate();
        }

	    GLFW.SwapInterval(1);
        
        while(!GLFW.WindowShouldClose(_windowHandle)) {
            OpenGL.Clear((uint) ClearBufferMask.ColorBufferBit | (uint)ClearBufferMask.DepthBufferBit);
            
            OpenGL.BindVertexArray(_vertexArray);
            ShaderProgram.PushShader(_basicShader.Id);

            OpenGL.EnableVertexAttribArray(0); // vertices
            OpenGL.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBuffer);
            OpenGL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);

            OpenGL.EnableVertexAttribArray(1); // colors
            OpenGL.BindBuffer(BufferTargetARB.ArrayBuffer, _colorBuffer);
            OpenGL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);
            
            _basicShader.SetUniform("modelViewProjection", Matrix4x4.Identity);

            var tex = new Texture2D("Resources/Images/Rabbit.jpg");
            tex.Bind(TextureUnit.Texture0);
            
            _basicShader.SetUniform("sampler0", 0);
            
            OpenGL.DrawArrays(PrimitiveType.Triangles, 0, (uint)_triangleVerts.Length);
            
            GLFW.SwapBuffers(_windowHandle);
            
            GLFW.PollEvents();
        }
        
        GLFW.Terminate();
    }

    public void Update() {
        GLFW.PollEvents();
    }
}