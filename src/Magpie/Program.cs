using Magpie.Graphics;
using Magpie.Graphics.Shaders;
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
    
    private static readonly float[] _vertices =
    [
        -1.0f,-1.0f,-1.0f,
        -1.0f,-1.0f, 1.0f,
        -1.0f, 1.0f, 1.0f,
        1.0f, 1.0f,-1.0f,
        -1.0f,-1.0f,-1.0f,
        -1.0f, 1.0f,-1.0f,
        1.0f,-1.0f, 1.0f,
        -1.0f,-1.0f,-1.0f,
        1.0f,-1.0f,-1.0f,
        1.0f, 1.0f,-1.0f,
        1.0f,-1.0f,-1.0f,
        -1.0f,-1.0f,-1.0f,
        -1.0f,-1.0f,-1.0f,
        -1.0f, 1.0f, 1.0f,
        -1.0f, 1.0f,-1.0f,
        1.0f,-1.0f, 1.0f,
        -1.0f,-1.0f, 1.0f,
        -1.0f,-1.0f,-1.0f,
        -1.0f, 1.0f, 1.0f,
        -1.0f,-1.0f, 1.0f,
        1.0f,-1.0f, 1.0f,
        1.0f, 1.0f, 1.0f,
        1.0f,-1.0f,-1.0f,
        1.0f, 1.0f,-1.0f,
        1.0f,-1.0f,-1.0f,
        1.0f, 1.0f, 1.0f,
        1.0f,-1.0f, 1.0f,
        1.0f, 1.0f, 1.0f,
        1.0f, 1.0f,-1.0f,
        -1.0f, 1.0f,-1.0f,
        1.0f, 1.0f, 1.0f,
        -1.0f, 1.0f,-1.0f,
        -1.0f, 1.0f, 1.0f,
        1.0f, 1.0f, 1.0f,
        -1.0f, 1.0f, 1.0f,
        1.0f,-1.0f, 1.0f,
        
        // tringle vertices
        
        0.0f, -1.0f,  0.0f,
        0.0f, -2.0f,  0.0f,
        0.0f,  1.0f,  0.0f,
    ];

    private static readonly float[] _colorData = [
        0.583f,  0.771f,  0.014f,
        0.609f,  0.115f,  0.436f,
        0.327f,  0.483f,  0.844f,
        0.822f,  0.569f,  0.201f,
        0.435f,  0.602f,  0.223f,
        0.310f,  0.747f,  0.185f,
        0.597f,  0.770f,  0.761f,
        0.559f,  0.436f,  0.730f,
        0.359f,  0.583f,  0.152f,
        0.483f,  0.596f,  0.789f,
        0.559f,  0.861f,  0.639f,
        0.195f,  0.548f,  0.859f,
        0.014f,  0.184f,  0.576f,
        0.771f,  0.328f,  0.970f,
        0.406f,  0.615f,  0.116f,
        0.676f,  0.977f,  0.133f,
        0.971f,  0.572f,  0.833f,
        0.140f,  0.616f,  0.489f,
        0.997f,  0.513f,  0.064f,
        0.945f,  0.719f,  0.592f,
        0.543f,  0.021f,  0.978f,
        0.279f,  0.317f,  0.505f,
        0.167f,  0.620f,  0.077f,
        0.347f,  0.857f,  0.137f,
        0.055f,  0.953f,  0.042f,
        0.714f,  0.505f,  0.345f,
        0.783f,  0.290f,  0.734f,
        0.722f,  0.645f,  0.174f,
        0.302f,  0.455f,  0.848f,
        0.225f,  0.587f,  0.040f,
        0.517f,  0.713f,  0.338f,
        0.053f,  0.959f,  0.120f,
        0.393f,  0.621f,  0.362f,
        0.673f,  0.211f,  0.457f,
        0.820f,  0.883f,  0.371f,
        0.982f,  0.099f,  0.879f
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
        fixed (void* v = &_vertices[0]) {
            OpenGL.BufferData(BufferTargetARB.ArrayBuffer, (nuint) (_vertices.Length * sizeof(uint)), v, BufferUsageARB.StaticDraw);
        }

        OpenGL.GenBuffers(1, out _colorBuffer);
        OpenGL.BindBuffer(BufferTargetARB.ArrayBuffer, _colorBuffer);
        fixed (void* c = &_colorData[0]) {
            OpenGL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(_colorData.Length * sizeof(float)), c, BufferUsageARB.StaticDraw);
        }

        if(GLFW.GetKey(_windowHandle, Keys.Escape) == (int)InputAction.Press) {
            GLFW.Terminate();
        }
        
        float lastTime = (float)GLFW.GetTime();

	GLFW.SwapInterval(1);
        
        while(!GLFW.WindowShouldClose(_windowHandle)) {
            OpenGL.Clear((uint) ClearBufferMask.ColorBufferBit | (uint)ClearBufferMask.DepthBufferBit);
            
            OpenGL.BindVertexArray(_vertexArray);
            ShaderProgram.PushShader(_basicShader.Id);
            
            // really bad and ugly, ignore
            var position = new Vector3(4.0f, 3.0f, -3.0f); 
            var horizontalAngle = 3.14f;
            var verticalAngle = 0.0f;
            var speed = 3.0f;
            var mouseSpeed = 0.005f;
            
            float currentTime = (float)GLFW.GetTime();
            float deltaTime = currentTime - lastTime;
            lastTime = currentTime;

            double mouseX, mouseY;
            GLFW.GetCursorPos(_windowHandle, out mouseX, out mouseY);
            GLFW.SetCursorPos(_windowHandle, _screen_width / 2, _screen_height / 2);

            horizontalAngle += mouseSpeed * (float)(_screen_width / 2 - mouseX);
            verticalAngle += mouseSpeed * (float)(_screen_height / 2 - mouseY);

            verticalAngle = Math.Clamp(verticalAngle, -MathF.PI / 2.0f, MathF.PI / 2.0f);

            Vector3 direction = new Vector3(
                MathF.Cos(verticalAngle) * MathF.Sin(horizontalAngle),
                MathF.Sin(verticalAngle),
                MathF.Cos(verticalAngle) * MathF.Cos(horizontalAngle)
            );

            Vector3 rightDir = new Vector3(
                MathF.Sin(horizontalAngle - MathF.PI / 2.0f),
                0,
                MathF.Cos(horizontalAngle - MathF.PI / 2.0f)
            );

            Vector3 up = Vector3.Cross(rightDir, direction);

            if (GLFW.GetKey(_windowHandle, Keys.W) == (int)InputAction.Press)
                position += direction * deltaTime * speed;
            if (GLFW.GetKey(_windowHandle, Keys.S) == (int)InputAction.Press)
                position -= direction * deltaTime * speed;
            if (GLFW.GetKey(_windowHandle, Keys.D) == (int)InputAction.Press)
                position += rightDir * deltaTime * speed;
            if (GLFW.GetKey(_windowHandle, Keys.A) == (int)InputAction.Press)
                position -= rightDir * deltaTime * speed;

            var view = Matrix4x4.CreateLookAt(position, position + direction, up);
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(
                MathF.PI / 4.0f,
                _screen_width / _screen_height,
                0.1f,
                100.0f
            );

            var model = Matrix4x4.Identity;
            var mvp = model * view * projection;
            
            _basicShader.SetUniform("modelViewProjection", mvp);

            OpenGL.EnableVertexAttribArray(0); // vertices
            OpenGL.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBuffer);
            OpenGL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);

            OpenGL.EnableVertexAttribArray(1); // colors
            OpenGL.BindBuffer(BufferTargetARB.ArrayBuffer, _colorBuffer);
            OpenGL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);

            OpenGL.DrawArrays(PrimitiveType.Triangles, 0, (uint)_vertices.Length / 3);
            
            var viewTriangle = Matrix4x4.CreateLookAt(
                new Vector3(3.0f, 2.0f, -2.0f),
                Vector3.Zero,
                new Vector3(0.0f, 1.0f, 0.0f) 
            );
            var mvp2 = model * viewTriangle * projection;
            
            _basicShader.SetUniform("modelViewProjection", mvp2);
            
            OpenGL.DrawArrays(PrimitiveType.Triangles, 34, 3);
            
            GLFW.SwapBuffers(_windowHandle);
            
            GLFW.PollEvents();
        }
        
        GLFW.Terminate();
    }

    public void Update() {
        GLFW.PollEvents();
    }
}