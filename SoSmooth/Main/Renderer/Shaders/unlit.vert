#version 330

uniform CameraData
{
    mat4 view;
    mat4 viewInv;
    mat4 proj;
    mat4 viewProj;
    vec3 position;
} cam;

uniform mat4 u_modelMatrix;
uniform vec4 u_color;

in vec3 v_position;
in vec4 v_color;

out vec4 f_color;

void main()
{
	gl_Position = cam.viewProj * u_modelMatrix * vec4(v_position, 1.0);
	f_color = u_color * v_color;
}