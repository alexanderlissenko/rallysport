#version 440
/* Copies incoming vertex color without change.
 * Applies the transformation matrix to vertex position.
 */

 in vec3 position;
 in vec3 normalIn;
 in vec2 textCoordIn;

 out vec4 colorOut;
 out vec2 textCoord;
 out vec3 viewSpaceNormal;
 out vec3 viewSpacePosition;

 uniform mat4 modelViewProjectionMatrix;
 uniform mat4 modelViewMatrix;
 uniform mat4 normalMatrix;



void main()
{
	viewSpaceNormal = (normalMatrix*vec4(normalIn,0.0)).xyz;
	viewSpacePosition = (modelViewMatrix *vec4(position,1)).xyz;

    gl_Position = modelViewProjectionMatrix*vec4(position,1); 
	textCoord = textCoordIn;
}