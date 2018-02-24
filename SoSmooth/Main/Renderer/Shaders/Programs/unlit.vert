out vec4 f_color;

void main()
{
	gl_Position = LocalToClipPos(v_position);
	f_color = u_color * v_color;
}