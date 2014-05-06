#version 420
/* Copies incoming vertex color without change.
 * Applies the transformation matrix to vertex position.
 */
 
uniform mat4 modelViewProjectionMatrix;

in vec3 positionIn;
out vec4 position;

void main() 
{
	position = modelViewProjectionMatrix*vec4(positionIn,1);
	gl_Position = position;
}
