#version 330 core

out vec4 FragColor;
in vec2 fUv;

void main() {
    FragColor = vec4(fUv.x, fUv.y, 1.0f, 1.0f);
}