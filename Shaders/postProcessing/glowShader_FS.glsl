#version 420

uniform sampler2D texture;
in vec2 pos;

out vec4 fragColor;

void main()
{
	vec4 color = texture2D(texture,pos.xy);
	color.x = color.x * color.w;
	color.y = color.y * color.w;
	color.z = color.z * color.w;
	
	fragColor = vec4(color.xyz,1);
}