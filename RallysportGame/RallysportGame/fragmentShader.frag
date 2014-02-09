#version 130
//almost the same as lab 3 from computer grafics 2013
// not tested


//  original code that should work
uniform sampler2D colortexture;
in vec4 colorOut;
in vec2 texCoord;
out vec4 fragmentColor;
///////////////////////////////

// new implemantations



////////////////////////////////


void main()
{
	fragmentColor = texture(colortexture, texCordIn.xy);
}