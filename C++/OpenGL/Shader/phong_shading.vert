#version 450 core

layout (location = 0) in vec3 vertexPosition;
layout (location = 1) in vec3 vertexNormal;
layout (location = 2) in vec2 vertexUV;

out vec4 position;
out vec3 normal;
out vec2 uv;

uniform mat4 object2world = mat4(1.0); // Model Matrix
uniform mat3 normal2world = mat3(1.0); // Inverse-Transpose Model Matrix
uniform mat4 object2clip = mat4(1.0); // MVP-Matrix

void main()
{
    uv = vertexUV;

    position = object2world * vec4(vertexPosition, 1.0);
    normal = normalize(normal2world * vertexNormal);
    gl_Position = object2clip * vec4(vertexPosition, 1.0);
}

