#version 420
//FS for skybox
precision highp float;


uniform sampler2D firstTexture;

in vec4 position;
in vec2 textureCoord;
out vec4 fragColor;

void main()
{	
	fragColor = texture(firstTexture, textureCoord);
}