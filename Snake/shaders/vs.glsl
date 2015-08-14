﻿#version 330
 
in vec2 vPosition;
in int in_type;

flat out int type;

uniform mat4 modelview;
 
void main()
{
    gl_Position = modelview * vec4(vPosition, 0.0f, 1.0f);
    type = in_type;
}