#version 330
 
in vec4 color;
flat in int type;
out vec4 outputColor;
 
void main()
{
	switch (type) {
		case -1: // Food
			outputColor = vec4(1.0f, 0.0f, 0.0f, 1.0f);
			break;
		case 0: // Empty
			outputColor = vec4(0.0f, 0.0f, 0.0f, 1.0f);
			break;
		default: // Body
			outputColor = vec4(0.0f, 1.0f, 1.0f, 1.0f);
			break;
	}
}