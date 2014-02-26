#version 130
/* Copies incoming fragment color without change. */

out vec4 fragmentColor;

void main() 
{
	fragmentColor = vec4(gl_FragCoord.z);//vec4(1.0);
}
