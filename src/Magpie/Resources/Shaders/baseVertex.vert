#version 330 core

layout (location = 0) in vec3 vPos;

out vec2 fragUv;
uniform mat4 modelViewProjection;

void main()
{
    gl_Position = modelViewProjection * vec4(vPos, 1.0f);
    fragUv = gl_Position.xy;
}