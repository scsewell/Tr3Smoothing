#version 330

uniform mat4 viewMatrix;

in vec3 f_worldPos;
in vec3 f_normal;
in vec4 f_color;

out vec4 fragColor;

void main()
{
	vec4 lightDiffuseColor = vec4(1.0, 1.0, 1.0, 1.0);
	vec4 lightSpecularColor = vec4(1.0, 1.0, 1.0, 1.0);
	vec3 lightDirection = -normalize(vec3(1.0, 1.0, 1.0));
	vec3 ambientLight = vec3(0.05, 0.05, 0.05);

	// renormalize as the interpolation only preserves direction but not magnitude correctly
	vec3 normal = normalize(f_normal);

	vec3 halfVector = normalize(-lightDirection + normalize(inverse(viewMatrix)[3].xyz - f_worldPos));
	vec3 specular = lightSpecularColor.rgb * pow(max(dot(normal, halfVector), 0.0), 100.0);

	// compute the diffuse lighting
	vec3 diffuse = ambientLight;
	float diffuseCoeff = max(dot(normal, -lightDirection), 0.0);
	diffuse += (diffuseCoeff * lightDiffuseColor.rgb);
	diffuse *= f_color.rgb;

	// gamma correct
	vec3 gamma = vec3(1.0/2.2);
    fragColor = vec4(pow(diffuse + specular, gamma), f_color.a);
}