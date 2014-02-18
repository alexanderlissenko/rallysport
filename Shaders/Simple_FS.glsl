#version 420
/* Copies incoming fragment color without change. */

in vec2 textCoord;
in vec3 viewSpaceNormal;
in vec3 viewSpacePosition;

out vec4 fragmentColor;

uniform vec3 viewSpaceLightPosition;

uniform float material_shininess = 25.0;
uniform vec3 material_diffuse_color = vec3(1.0); 
uniform vec3 material_specular_color = vec3(0.0); 
uniform vec3 material_emissive_color = vec3(0.0); 

//constants
uniform vec3 scene_ambient_light = vec3(0.2,0.2,0.2);
uniform vec3 scene_light = vec3(0.6,0.6,0.6);

vec3 calculateDiffuse(vec3 diffuseLight, vec3 materialDiffuse, vec3 normal, vec3 directionToLight)
{
	return diffuseLight *materialDiffuse* max(0,dot(normal,directionToLight));
}
vec3 calculateSpecular(vec3 specularLight, vec3 materialSpecular, float materialShininess, vec3 normal, vec3 directionToLight, vec3 directionFromEye)
{
	vec3 h = normalize(directionToLight - directionFromEye);
	float normalizeFactor = ((materialShininess+2.0)/8.0);
	return specularLight * materialSpecular* pow(max(0,dot(h,normal)), materialShininess)*normalizeFactor;
}
vec3 calculateFresnel(vec3 materialSpecular, vec3 normal, vec3 directionFromEye) 
{
	return materialSpecular + (vec3(1.0)-materialSpecular)*pow(clamp(1.0+dot(directionFromEye,normal),0.0,1.0),5.0);
}
void main()
{
	vec3 normal = normalize(viewSpaceNormal);
	vec3 directionToLight = normalize(viewSpaceLightPosition-viewSpacePosition);
    vec3 directionFromEye = normalize(viewSpacePosition);
	
	vec3 diffuse = material_diffuse_color;//vec3(1.0);
	vec3 specular = material_specular_color;//vec3(0.5);
	vec3 emissive = material_emissive_color;//vec3(0.0);

	vec3 fresnelSpecular = calculateFresnel(specular,normal, directionFromEye);

	vec3 shading = (vec3(1.0,0,0)*scene_ambient_light)
					+ calculateDiffuse(scene_light,diffuse,normal,directionToLight)
					+ calculateSpecular(scene_light, fresnelSpecular,material_shininess,normal,directionToLight,directionFromEye)
					+ emissive;

	fragmentColor = vec4(shading,1.0);

}