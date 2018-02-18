#version 330

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projMatrix;

uniform vec4 color;

in vec3 v_position;
in vec3 v_normal;
in vec4 v_color;

out vec3 f_worldPos;
out vec3 f_normal;
out vec4 f_color;

void main()
{
	gl_Position = projMatrix * viewMatrix * modelMatrix * vec4(v_position, 1.0);
	f_worldPos = (modelMatrix * vec4(v_position, 1.0)).xyz;
	f_normal = (modelMatrix * vec4(v_normal, 0.0)).xyz;
	f_color = color * v_color;
}