#version 130
/* Copies incoming fragment color without change. */
precision highp float;

in vec2 texCoord;
out vec4 fragmentColor;
uniform sampler2D diffuseTex;

void main() 
{
	fragmentColor = texture(diffuseTex, texCoord);
}
