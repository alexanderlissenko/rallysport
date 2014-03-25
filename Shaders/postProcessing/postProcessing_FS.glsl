#version 420
/* Copies incoming fragment color without change. */
precision highp float;

uniform sampler2D postTex;
uniform sampler2D postVel;
uniform float velScale;

in vec2 pos;

out vec4 fragColor;

void main() 
{
	vec2 texelSize = 1.0/vec2(textureSize(postTex,0));
	
	vec2 velocity = texture(postVel, pos).xy;//texture(postVel, screenTexCoords).xy;
	velocity = pow(velocity, vec2(1.0/3.0));
	velocity = velocity * 2.0 - 1.0; 
	velocity *= velScale;//4.8;// 
	
	float speed = length(velocity / texelSize);
	int nSamples = clamp(int(speed),1,20);
	
	vec2 texCoord = pos;
	
	vec4 result = texture(postTex, texCoord);
	
	for(int i = 1; i < nSamples; i++)
	{
		vec2 offset = velocity *(float(i)/float(nSamples-1)-0.5);
		result += texture(postTex,pos+offset);
	}
	
	result /= float(nSamples);
	
	fragColor = result;//vec4(velocity,0,1);//
}
