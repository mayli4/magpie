#version 330 core

layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec3 vertexColor;
layout (location = 2) in vec2 vertexTexCoord;

out vec3 fragColor;
out vec2 texCoord;

uniform mat4 modelViewProjection;

void main() {
    gl_Position = modelViewProjection * vec4(vertexPosition, 1.0);
    fragColor = vertexColor;
    
    texCoord = vertexPosition.xy;
}