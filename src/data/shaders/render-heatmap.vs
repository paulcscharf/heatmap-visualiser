#version 150

uniform mat4 projectionMatrix;
uniform mat4 modelviewMatrix;

in vec3 v_position;
in vec2 v_uv;

out vec2 p_uv;

void main()
{
	gl_Position = projectionMatrix * modelviewMatrix * vec4(v_position, 1.0);
	p_uv = v_uv;
}