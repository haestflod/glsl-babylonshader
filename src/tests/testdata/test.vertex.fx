attribute vec3 aPosition;
attribute vec2 aUV;

varying vec3 vPosition;
varying vec2 vUV;

uniform vec4 uForm; // after variable linecomment
		
// Main function!
void main(void)
{
#ifdef PREPROCESSOR
	vec4 testcolor = vec4(1.0, 1.0, 1.0, 1.0);
#endif


	/****** block comment */
	if (vUV != 1.0 ) {
		/****** another block comment ******/
		float test = 1.0;
	} else {
		float test2 = 2.0;
	}

	float x = 1.0; /* comment *//* 2ndComment */ float y = 1.0; /* ThirdComment*/ // float z = 1.0;
	// float z = 2.0;
	/*


	Multi line comment
	float z = 3.0;
	*/ float z = 4.0; // After line comment
	

	

	// Good thing we're not validating output then this would crash :p
	gl_Position = vPos;
}
