#version 130

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projMatrix;

in vec3 v_position;
in vec3 v_normal;
in vec4 v_color;

out vec4 p_color;

void main()
{
	gl_Position = projMatrix * viewMatrix * modelMatrix * vec4(v_position, 1.0);
	p_color = v_color;
}