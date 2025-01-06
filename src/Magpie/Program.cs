using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.GLFW;

namespace Magpie;

// ReSharper disable InconsistentNaming
unsafe class Program {
    static void Main(
        string[] args) {
        var magpie = new Magpie();
        magpie.Initialize();
    }
}

public unsafe class Magpie {
    private const int _screen_height = 800;
    private const int _screen_width = 600;
    private const string _window_title = "magpie";

    private GL _openGL;
    private IWindow _window;

    public Glfw GLFW;
    
    private GlfwContext _glfwContext;
    private GlfwContext _glContext => _glfwContext;
    private WindowHandle* _windowHandle { get; set; }
    
    private static readonly Version _defaultOpenGLVersion = new(3, 2);

    private float[] _vertices = {
        -1.0f, -1.0f, 0.0f,
        1.0f, -1.0f, 0.0f,
        0.0f,  1.0f, 0.0f
    };
    
    private uint _vertexBuffer;
    private uint _testShader;
    
    //Vertex shaders are run on each vertex.
    private static readonly string _vertexShaderSource = @"
        #version 330 core //Using version GLSL version 3.3
        layout (location = 0) in vec4 vPos;
        
        void main()
        {
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }
        ";

    //Fragment shaders are run on each fragment/pixel of the geometry.
    private static readonly string _fragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
        }
        ";
    
    public void Initialize() {
        GLFW = Glfw.GetApi();
        
        if(!GLFW.Init()) {
            throw new Exception("GLFW Initialization failed.");
        }
        Console.WriteLine("GLFW Initialization success.");
        
        GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 3);
        GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 2);
        GLFW.WindowHint(WindowHintBool.OpenGLForwardCompat, true);
        GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);

        _windowHandle = GLFW.CreateWindow(_screen_width, _screen_height, _window_title, null, null);

        _glfwContext = new(GLFW, _windowHandle);
        GLFW.MakeContextCurrent(_windowHandle);
        _openGL = GL.GetApi(_glContext);
        
        _vertexBuffer = _openGL.GenVertexArray();
        _openGL.BindVertexArray(_vertexBuffer);
        fixed(void* v = &_vertices[0]) {
            _openGL.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (_vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw);
        }

        var vertShader = _openGL.CreateShader(ShaderType.VertexShader);
        _openGL.ShaderSource(vertShader, _vertexShaderSource);
        _openGL.CompileShader(vertShader);
        var infoLogVert = _openGL.GetShaderInfoLog(vertShader);
        if (!string.IsNullOrWhiteSpace(infoLogVert)) {
            Console.WriteLine($"Error compiling vertex shader {infoLogVert}");
        }
        
        var fragShader = _openGL.CreateShader(ShaderType.FragmentShader);
        _openGL.ShaderSource(fragShader, _fragmentShaderSource);
        _openGL.CompileShader(fragShader);
        var infoLogFrag = _openGL.GetShaderInfoLog(vertShader);
        if (!string.IsNullOrWhiteSpace(infoLogFrag)) {
            Console.WriteLine($"Error compiling fragment shader {infoLogFrag}");
        }
        
        _testShader = _openGL.CreateProgram();
        _openGL.AttachShader(_testShader, vertShader);
        _openGL.AttachShader(_testShader, fragShader);
        _openGL.LinkProgram(_testShader);

        //Checking the linking for errors.
        _openGL.GetProgram(_testShader, GLEnum.LinkStatus, out var status);
        if (status == 0)
        {
            Console.WriteLine($"Error linking shader {_openGL.GetProgramInfoLog(_testShader)}");
        }
        
        _openGL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
        _openGL.DisableVertexAttribArray(0);
        
        while(!GLFW.WindowShouldClose(_windowHandle)) {
            _openGL.ClearColor(0, 0, 0, 1);
            _openGL.Clear(ClearBufferMask.ColorBufferBit);
            
            GLFW.SwapBuffers(_windowHandle);
            GLFW.PollEvents();
        }
        
        GLFW.Terminate();
    }

    public void Update() {
        GLFW.PollEvents();
    }
}