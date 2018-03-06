in vec3 f_worldPos;
in vec3 f_normal;
in vec4 f_color;

void main()
{
	// renormalize as the interpolation only preserves direction but not magnitude correctly
	vec3 normal = normalize(f_normal);

	vec3 diffuse = light.ambient;
    vec3 specular = vec3(0.0);

    // compute the lighting contribution from each light
    for (int i = 0; i < LIGHT_COUNT; i++)
    {
        vec3 lightNormal = -light.direction[i];
        vec3 halfVector = normalize(lightNormal + normalize(cam.position - f_worldPos));

        diffuse += light.diffColor[i] * max(dot(normal, lightNormal), 0.0);
        specular += light.specColor[i] * pow(max(dot(normal, halfVector), 0.0), 50.0);
    }

	diffuse *= f_color.rgb;

	// gamma correct
	vec3 gamma = vec3(1.0/2.2);
    gl_FragColor = vec4(pow(diffuse + specular, gamma), f_color.a);
}