shader_type canvas_item;

uniform sampler2D screen_texture:hint_screen_texture;

void vertex() {
	// Called for every vertex the material is visible on.
	//COLOR = texture(TEXTURE, UV); 
	}

void fragment() {
	COLOR = texture(TEXTURE,UV);
	
	COLOR *= texture(screen_texture,SCREEN_UV);
	// Set the output color to the texture
	//COLOR = texture(LightMask, UV); 
	// Called for every pixel the material is visible on.
	//float x_coord = UV.x*TileBounds.x;
	//x_coord = floor(x_coord);
	//COLOR.r = COLOR.r * x_coord;
	//COLOR = vec4(UV, 0.5, 1.0);
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
