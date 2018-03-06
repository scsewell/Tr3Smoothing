out vec4 f_color;

void main()
{
	gl_Position = LocalToClipPos(v_position);
	f_color =  vec4(1.0, 0.0, 1.0, 1.0);
}