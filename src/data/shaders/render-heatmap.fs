#version 150


uniform sampler2D heatmap;
uniform sampler2D spectrum;

in vec2 p_uv;

out vec4 fragColor;

void main()
{
	float sample = texture(heatmap, p_uv).x;

	sample = atan(sample) / 1.57;

	vec4 argb = texture(spectrum, vec2(sample, 0.5));

    fragColor = argb;
}