#version 420
//FS for godrays
precision highp float;


uniform int isLight;

in vec4 position;

out vec4 fragColor;

void main()
{
	vec3 outColor;
	if (isLight == 1)
	{
		//render white
		outColor = vec3(1);
	}
	else 
	{
		//render black
		outColor = vec3(0);
	}
	
	fragColor = vec4(outColor,1);
}