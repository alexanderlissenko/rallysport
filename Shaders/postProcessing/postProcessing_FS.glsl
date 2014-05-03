#version 420
/* Copies incoming fragment color without change. */
precision highp float;

uniform sampler2D postTex;
uniform sampler2D postVel;
uniform sampler2D postDepth;
uniform sampler2D megaPartTex;
uniform sampler2D megaPartDepth;
uniform sampler2D glowTexture;
uniform float velScale;
uniform vec3 lightPos;

in vec2 pos;


out vec4 fragColor;

void main() 
{
    
    
    vec4 result;
	vec2 texelSize = 1.0/vec2(textureSize(postTex,0));
	float depth = texture(postDepth,pos).x;
    float depth2 = texture(megaPartDepth,pos).x;
    if(depth > depth2)
    {
    result = texture(megaPartTex, pos);
    }
    else{
    result = texture(postTex, pos);
   
	vec2 velocity = texture(postVel, pos).xy;//texture(postVel, screenTexCoords).xy;
	velocity = pow(velocity, vec2(1.0/3.0));
	velocity = velocity * 2.0 - 1.0; 
	velocity *= velScale;//4.8;// 
	
	float speed = length(velocity / texelSize);
	int nSamples = clamp(int(speed),1,20);
	
	
	
	float offsetDepth;
	for(int i = 1; i < nSamples; i++)
	{
		vec2 offset = velocity *(float(i)/float(nSamples-1)-0.5);
		offsetDepth = texture(postDepth,pos+offset).x;
		float weight = 1-(offsetDepth-depth);
		result += texture(postTex,pos+offset);//*weight;
	}
	
	result /= float(nSamples);
	}
	//fragColor = result;//vec4(velocity,0,1);//
    
	//viewspace godrays
	int NUM_SAMPLES = 10;
	float Exposure = 2.0;
	float Density = 0.5;
	float Weight = 0.5;
	float Decay = 0.5;
	
	vec2 tmpPos = pos;
	
	vec2 deltaTexCoord = (pos.xy - lightPos.xy);
	deltaTexCoord *= 1.0 / NUM_SAMPLES * Density;			//NUM_SAMPLES & Density undefined
	
	//vec3 color = texture2D(postTex, pos.xy);		
	vec3 color = result.xyz;
	float illuminationDecay = 1.0;
	vec3 sample2 = vec3(0);
	
	for (int i = 0; i < NUM_SAMPLES; i++) 					//NUM_SAMPLES
	{
		tmpPos.xy -= deltaTexCoord;
		sample2 = texture2D(postTex, tmpPos.xy).xyz;	
		sample2 *= illuminationDecay * Weight;				//Weight
		
		color += sample2;
		illuminationDecay *= Decay;
	}
	
	vec4 godrayRes = vec4(color * Exposure, 1);					//Exposure
	//godrays end
	
	vec4 glow = texture2D(glowTexture,pos );
    //result = (result + godrayRes) / 2;
    fragColor = result+glow;// 
}
