
Shader "Custom/AlphaSelfIllum" {
		Properties{
			_Color("Main Color", Color) = (1,1,1,0)
			_SpecColor("Spec Color", Color) = (1,1,1,1)
			_Emission("Emmisive Color", Color) = (0,0,0,0)
			_Shininess("Shininess", Range(0.01, 1)) = 0.7
			_MainTex("Base (RGB)", 2D) = "white" { }
		}

		SubShader{
		Tags{ "QUEUE" = "AlphaTest" "IGNOREPROJECTOR" = "true" "RenderType" = "TransparentCutout" }
		// We use the material in many passes by defining them in the subshader.
		// Anything defined here becomes default values for all contained passes.
		Material{
			Diffuse[_Color]
			Ambient[_Color]
			Shininess[_Shininess]
			Specular[_SpecColor]
			Emission[_Emission]
		}
		ZWrite On
		Lighting On
		SeparateSpecular On

		// Set up alpha blending
		Blend SrcAlpha OneMinusSrcAlpha

		// Render the back facing parts of the object.
		// If the object is convex, these will always be further away
		// than the front-faces.  
		//控制前面透明度      
		Pass {
		    Cull Front
		    SetTexture [_MainTex] {
		        Combine Primary * Texture
		    }
		}
		// Render the parts of the object facing us.
		// If the object is convex, these will be closer than the
		// back-faces.
		//控制后面透明度  
		Pass{
			Cull back
			SetTexture[_MainTex]{
				constantColor(1,1,1,1)
				combine constant lerp(texture) previous
			}
			SetTexture[_MainTex]{
				Combine Primary * Texture
			}
		}
	}
}
