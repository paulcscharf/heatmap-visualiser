#version 150

uniform sampler2D diffuse;

uniform vec2 sampleStep;

in vec2 p_texCoord;

out vec4 fragColor;

void main()
{
	vec2 uv = p_texCoord;
	float a = texture(diffuse, uv).x * 2;

	a += texture(diffuse, uv + sampleStep).x;
	a += texture(diffuse, uv - sampleStep).x;

	a /= 4.01;

	fragColor = vec4(a, 0, 0, 1);
}