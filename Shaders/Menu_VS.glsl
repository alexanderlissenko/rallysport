#version 130
/* Copies incoming vertex color without change.
 * Applies the transformation matrix to vertex position.
 */

in vec3 positionIn;
in vec2 texCoordIn;
out vec2 texCoord;

void main() 
{
	gl_Position = vec4(positionIn,1.0);
	texCoord = vec2(positionIn.x,-positionIn.y)*0.5+0.5;
}
