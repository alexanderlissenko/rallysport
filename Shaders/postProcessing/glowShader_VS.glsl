#version 420
/* Copies incoming vertex color without change.
 * Applies the transformation matrix to vertex position.
 */

in vec3 positionIn;
out vec2 pos;

void main() 
{
	pos = positionIn.xy;
	gl_Position = vec4(positionIn,1)*2-1;
}
