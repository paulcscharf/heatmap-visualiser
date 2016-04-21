#version 150

in vec2 p_uv;

out vec4 fragColor;

void main()
{
	vec2 diff = (p_uv - vec2(0.5, 0.5)) * 2;
	float dSquared = dot(diff, diff);

	if (dSquared > 1)
		discard;

	float a = 1 - dSquared;

	a *= 0.2;

    fragColor = vec4(a, 0, 0, 1);
}