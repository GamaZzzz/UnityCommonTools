Shader "Self_Illumination" {	Properties{		_IlluminCol("Self-Illumination color (RGB)", Color) = (0.64,0.64,0.64,1)		_MainTex("Base (RGB) Self-Illumination (A)", 2D) = "white" {}	}	SubShader{		Tags{ "QUEUE" = "AlphaTest" "IGNOREPROJECTOR" = "true" "RenderType" = "TransparentCutout" }		Lighting On
		SeparateSpecular On

		// Set up alpha blending
		Blend SrcAlpha OneMinusSrcAlpha		Pass{			Tags{ "QUEUE" = "AlphaTest" "IGNOREPROJECTOR" = "true" }			Material{				Ambient(1,1,1,1)				Diffuse(1,1,1,1)			}			Lighting On			Cull Back
							SetTexture[_MainTex]{
				constantColor(1,1,1,1)
				combine constant lerp(texture) previous
			}			SetTexture[_MainTex]{ ConstantColor[_IlluminCol] combine constant * texture }			SetTexture[_MainTex]{ combine previous + texture }			SetTexture[_MainTex]{ ConstantColor[_IlluminCol] combine previous * constant }		}	}}