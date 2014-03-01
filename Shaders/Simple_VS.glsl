#version 130
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
 out vec4 shadowMapCoord;

 uniform mat4 modelViewProjectionMatrix;
 uniform mat4 modelViewMatrix;
 uniform mat4 normalMatrix;
 uniform mat4 lightMatrix;

 uniform mat4 lightproj;
 uniform mat4 viewMatrix;


void main()
{
	viewSpaceNormal = (normalMatrix*vec4(normalIn,0.0)).xyz;
	viewSpacePosition = (modelViewMatrix *vec4(position,1.0)).xyz;

	//mat4 temp = lightproj * lightMatrix * inverse(viewMatrix);

	shadowMapCoord = lightMatrix *modelViewMatrix * vec4(position,1.0);//vec4(viewSpacePosition,1.0);
	//shadowMapCoord.xyz *= vec3(0.5,0.5,0.5);
	//shadowMapCoord.xyz += shadowMapCoord.w * vec3(0.5,0.5,0.5);


    gl_Position = modelViewProjectionMatrix*vec4(position,1); 
	textCoord = textCoordIn;
}