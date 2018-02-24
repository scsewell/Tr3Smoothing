out vec3 f_worldPos;
out vec3 f_normal;
out vec4 f_color;

void main()
{
	gl_Position = LocalToClipPos(v_position);
	f_worldPos = LocalToWorldPos(v_position);
	f_normal = NormalToWorld(v_normal);
	f_color = u_color * v_color;
}