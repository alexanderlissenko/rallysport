
/* Copies incoming fragment color without change. */

in vec2 textCoord;
in vec3 viewSpaceNormal;
in vec3 viewSpacePosition;

uniform vec3 viewSpaceLightPosition;

//constants
uniform vec3 scene_ambient_light = vec3(0.1,0.1,0.1);
uniform vec3 scene_light = vec3(0.6,0.6,0.6);

vec3 calculateDiffuse(vec3 diffuseLight, vec3 materialDiffuse, vec3 normal, vec3 directionToLight)
{
	return diffuseLight *materialDiffuse* max(0,dot(normal,directionToLight));
}
vec3 calculateSpecular(vec3 specularLight, vec3 materialSpecular, float materialShininess, vec3 normal, vec3 directionToLight, vec3 directionFromEye)
{
	vec3 h = normalize(directionToLight - directionFromEye);
	return specularLight * materialSpecular* pow(max(0,dot(h,normal)), materialShininess);
}
void main()
{
	vec3 normal = normalize(viewSpaceNormal);
	vec3 directionToLight = normalize(viewSpaceLightPosition-viewSpacePosition);
    vec3 directionFromEye = normalize(viewSpaceNormal);

	
	vec3 diffuse = vec3(0.7);
	vec3 specular = vec3(0.5);
	float material_shininess = 0.10;
	
	vec3 shading = (vec3(1.0,0,0)*scene_ambient_light)
					+ calculateDiffuse(scene_light,diffuse,normal,directionToLight)
					+ calculateSpecular(scene_light, specular,material_shininess,normal,directionToLight,directionFromEye);

	gl_FragColor = vec4(shading,1.0);

}