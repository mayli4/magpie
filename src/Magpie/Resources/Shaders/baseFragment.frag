#version 330 core

out vec4 FragColor;
in vec2 fragUv;

void main() {
    FragColor = vec4(fragUv.x, fragUv.y, 1.0f, 1.0f);
}