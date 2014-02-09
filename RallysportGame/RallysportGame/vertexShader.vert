//simplest form of shader ver 1.0
//almost the same as lab 3 from computer grafics 2013


in vec3		position;
in vec3		colorIn;
out vec4	colorOut;
in	vec2	texCoordIn;	// incoming texcoord from the texcoord array
out	vec2	texCoord;	// outgoing interpolated texcoord to fragshader




uniform mat4 modelViewMatrix;

void main()
{
	position = modelViewMatrix* vec4(posiition,1); // model to view space for eatch vertex
	colorOut = vec4(colorIn,1);
	texCoord = texCoordIn; 
}