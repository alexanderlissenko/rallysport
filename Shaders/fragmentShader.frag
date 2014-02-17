#version 130

in vec4 colorOut;
in vec2 texCoord;
out vec4 fragmentColor;

uniform vec3 lightPosition;
uniform sampler2D colortexture;

vec3 calculateAmbient(vec3 light, vec3 color)
{
	return light * color;
}

vec3 calculateDiffuse(vec3 light, vec3 color, vec3 normal, vec3 directionToLight)
{
	return light * color * max(0, dot(normal, directionToLight));
}

vec3 calculateSpecular(vec3 specularLight, vec3 materialSpecular, float materialShininess, vec3 normal, vec3 directionToLight, vec3 directionFromEye)
{
	vec3 h = normalize(directionToLight - directionFromEye);
	float normalizationFactor = ((materialShininess + 2.0) / 8.0);
	return specularLight * materialSpecular * pow(max(0, dot(h, normal)), materialShininess) * normalizationFactor;
}

vec3 calculateFresnel(vec3 materialSpecular, vec3 normal, vec3 directionFromEye)
{
	return materialSpecular + (vec3(1.0) - materialSpecular) * pow(clamp(1.0 + dot(directionFromEye, normal), 0.0, 1.0), 5.0);
}





void main()
{
	//fragmentColor = texture(colortexture, texCordIn.xy);
	fragmentColor = vec4(1.0, 0.0, 0.0, 1.0);
}