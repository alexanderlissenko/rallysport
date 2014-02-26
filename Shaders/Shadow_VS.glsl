#version 130
/* Copies incoming vertex color without change.
 * Applies the transformation matrix to vertex position.
 */

in vec3 position;
uniform mat4 modelViewProjectionMatrix; 

void main() 
{
	gl_Position = modelViewProjectionMatrix * vec4(position,1.0);
}
