#version 330 core

layout (location = 0) in vec4 vPos;

out vec2 fUv;

void main()
{
    gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
    fUv = gl_Position.xy;
}