//simplest form of shader ver 1.0

in vec3		position;
in vec3		colorIn;
out vec4	colorOut;
in	vec2	texCoordIn;	// incoming texcoord from the texcoord array
out	vec2	texCoord;	// outgoing interpolated texcoord to fragshader
in vec3		normalIn;
out vec3	viewSpacePosition;
out vec3	viewSpaceNormal;
out vec3	viewSpaceLightPosition;

// Uniforms that may or may not be useful
uniform mat4 modelMatrix; 
uniform mat4 viewMatrix; 
uniform mat4 projectionMatrix; 
uniform mat4 modelViewMatrix;
uniform mat4 modelViewProjectionMatrix;
uniform mat4 normalMatrix //inverse(transpose(modelViewMatrix));
uniform vec3 lightpos; 
uniform mat4 lightMatrix;


void main()
{
	position = modelViewMatrix* vec4(position,1); // model to view space for each vertex
	colorOut = vec4(colorIn,1);
	texCoord = texCoordIn; 
}