using Silk.NET.OpenGL;
using Silk.NET.GLFW;

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

    private GL _openGL;

    public Glfw GLFW;
    
    private GlfwContext _glfwContext;
    private GlfwContext _glContext => _glfwContext;
    private WindowHandle* _windowHandle { get; set; }

    private static uint _vertexBuffer;
    private static uint _vertexArray;
    private static uint _testShader;

    //Vertex shaders are run on each vertex.
    private static readonly string _vertexShaderSource = @"
        #version 330 core //Using version GLSL version 3.3
        layout (location = 0) in vec4 vPos;

        out vec2 fUv;
        
        void main()
        {
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
            fUv = gl_Position.xy;
        }
        ";

    //Fragment shaders are run on each fragment/pixel of the geometry.
    private static readonly string _fragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
        in vec2 fUv;

        void main()
        {
            float dist = distance(fUv, vec2(0.5f, 0.5f));
            vec2 middif = (fUv - vec2(0.5f, 0.5f));
            vec2 uv = fUv;

            uv += dist * dist * dist * dist * dist * dist * dist * middif * 300.0;

            FragColor = vec4(uv.x, uv.y, 1.0f, 1.0f);
        }
        ";
    
    private static readonly float[] _vertices = {
        -1.0f, -1.0f, 0.0f,
        1.0f, -1.0f, 0.0f,
        0.0f,  1.0f, 0.0f,
    };
    
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
        
        // hello triangle

        _glfwContext = new(GLFW, _windowHandle);
        GLFW.MakeContextCurrent(_windowHandle);
        _openGL = GL.GetApi(_glContext);
        
        _vertexArray = _openGL.GenVertexArray();
        _openGL.BindVertexArray(_vertexArray);
        _vertexBuffer = _openGL.GenBuffer();
        _openGL.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBuffer);
        fixed (void* v = &_vertices[0]) {
            _openGL.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (_vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw); //Setting buffer data.
        }

        //create vert shader
        uint vertexShader = _openGL.CreateShader(ShaderType.VertexShader);
        _openGL.ShaderSource(vertexShader, _vertexShaderSource);
        _openGL.CompileShader(vertexShader);

        //create frag shader
        uint fragmentShader = _openGL.CreateShader(ShaderType.FragmentShader);
        _openGL.ShaderSource(fragmentShader, _fragmentShaderSource);
        _openGL.CompileShader(fragmentShader);
            

        _testShader = _openGL.CreateProgram();
        _openGL.AttachShader(_testShader, vertexShader);
        _openGL.AttachShader(_testShader, fragmentShader);
        _openGL.LinkProgram(_testShader);

        _openGL.DetachShader(_testShader, vertexShader);
        _openGL.DetachShader(_testShader, fragmentShader);
        _openGL.DeleteShader(vertexShader);
        _openGL.DeleteShader(fragmentShader);

        //tell opengl how to give the data to shaders
        _openGL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
        _openGL.EnableVertexAttribArray(0);
        
        while(!GLFW.WindowShouldClose(_windowHandle)) {
            _openGL.Clear((uint) ClearBufferMask.ColorBufferBit);

            _openGL.BindVertexArray(_vertexArray);
            _openGL.UseProgram(_testShader);

            _openGL.DrawArrays(PrimitiveType.Triangles, 0, (uint)_vertices.Length);
            
            GLFW.SwapBuffers(_windowHandle);
            GLFW.PollEvents();
        }
        
        GLFW.Terminate();
    }

    public void Update() {
        GLFW.PollEvents();
    }
}