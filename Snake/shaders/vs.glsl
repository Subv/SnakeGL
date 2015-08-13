#version 330
 
in vec3 vPosition;
in int in_type;

out vec4 color;
flat out int type;

uniform mat4 modelview;
 
void main()
{
    gl_Position = modelview * vec4(vPosition, 1.0);
    color = vec4(0.0f, 0.0f, 0.0f, 1.0f);
    type = in_type;
}