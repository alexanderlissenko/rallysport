#version 130
/* Copies incoming fragment color without change. */
precision highp float;

out vec4 fragmentColor;

void main() 
{
	fragmentColor = vec4(1.0);//vec4(gl_FragCoord.z);//
}
