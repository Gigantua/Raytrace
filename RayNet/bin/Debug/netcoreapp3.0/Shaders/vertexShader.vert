#version 440 core

layout (location = 0) in float time;
layout (location = 1) in vec2 Screen;

layout (location = 2) in vec3 cam_pos1;
layout (location = 3) in vec3 cam_forward1;
layout (location = 4) in vec3 cam_up1;
layout (location = 5) in vec3 cam_right1;

out vec2 SCREEN;
out float TIME;

out vec3 cam_pos;
out vec3 cam_forward;
out vec3 cam_up;
out vec3 cam_right;

const vec2 quadVertices[4] = 
{ 
	vec2(-1.0, -1.0), 
	vec2(1.0, -1.0), 
	vec2(-1.0, 1.0), 
	vec2(1.0, 1.0) 
};

void main()
{
	TIME = time;
	SCREEN = Screen;
    gl_Position = vec4(quadVertices[gl_VertexID], 0.0, 1.0);
	
	cam_pos = cam_pos1;
	cam_forward = cam_forward1;
	cam_up = cam_up1;
	cam_right = cam_right1;
}
