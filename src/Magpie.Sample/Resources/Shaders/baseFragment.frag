#version 330 core

in vec3 fragColor;
out vec4 finalColor;

in vec2 texCoord;

uniform sampler2D sampler0;

void main() {
    finalColor = vec4(fragColor, 1.0) * texture(sampler0, texCoord);
}