#version 330
 
flat in int type;
out vec4 outputColor;

uniform int length;
 
void main()
{
    switch (type) {
        case -1: // NormalFood
            outputColor = vec4(1.0f, 0.0f, 0.0f, 1.0f);
            break;
        case -2: // ShrinkageFood
            outputColor = vec4(0.0f, 0.6f, 0.0f, 0.8f);
            break;
        case 0: // Empty
            outputColor = vec4(0.0f, 0.0f, 0.0f, 1.0f);
            break;
        default: // Body
            outputColor = vec4(0.0f, 2.0f * float(type) / float(length), 2.0f * float(type) / float(length), 1.0f);
            break;
    }
}