// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "LexLiu/VertexColorTransform" //顶点颜色变换
{
	Properties
	{
		_MainColor("Main Color", Color) = (1,1,1,1)
		_CubeMap("Cube Map", CUBE) = ""{}
		_ReflectAmount("Reflect Amount", Float) = 1
		_RimColorMultiply("Rim Color Multiply", Float) = 0
		_RimPower("Rim Power", Float) = 1
		_ReflFresnelPower("Reflect Fresnel Power", Float) = 1
		_ReflFresnelOffset("Reflect Fresnel Offset", Float) = 0
		_WaveLength("Wave Length", Float) = 1
		_WaveStrenth("Wave Strenth", Float) = 1
		_WaveOffset("Wave Offset", Float) = 0
		_StartPos("Start Position", Vector) = (0,0,0,0)
		_TargetColor("Target Color", Color) = (1,1,1,1)
		_OcclusionStrength("DarkColor Strength", Float) = 1
		_OcclusionOffset("DarkColor Offset", Float) = 0
		_FinalColorAdjust("FinalColorAdjust", Float) = 1.3
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" "IgnoreProjector" = "True" }
		CGINCLUDE
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma exclude_renderers xbox360 ps3 flash d3d11_9x
		fixed4 _MainColor;
	samplerCUBE _CubeMap;
	fixed _ReflectAmount;
	half _ReflFresnelPower;
	half _ReflFresnelOffset;
	half _RimColorMultiply;
	half _RimPower;
	half _WaveLength;
	half _WaveStrenth;
	half _WaveOffset;
	float4 _StartPos;
	fixed4 _TargetColor;
	fixed _OcclusionStrength;
	float _OcclusionOffset;
	float _FinalColorAdjust;

	fixed4 _LightColor0;
	struct VertexInput
	{
		float4 vertex:POSITION;
		fixed3 normal : NORMAL;
	};
	struct VertexOutput
	{
		float4 pos:SV_POSITION;
		fixed4 reflectDir : TEXCOORD0;
		fixed3 lightColor : TEXCOORD1;
		fixed rimMultiply : TEXCOORD2;
		fixed3 transColor : TEXCOORD3;
		float4 worldPos:TEXCOORD4;
	};
	VertexOutput vert(VertexInput v)
	{
		VertexOutput output;
		//vertex offset
		fixed pi = 3.1415926535898;
		half dist = distance(_StartPos.xyz, mul(unity_ObjectToWorld, v.vertex).xyz);
		half x = (_WaveOffset - dist) * _WaveLength;
		half normalOffset = (sin(x + pi*0.5) + 1) * _WaveStrenth;
		normalOffset = (x > -pi && x < pi) ? normalOffset : 0;
		float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
		worldPos.xyz += float3(mul(unity_ObjectToWorld, fixed4(v.normal,0)).xyz) * normalOffset;
		output.pos = mul(UNITY_MATRIX_VP, worldPos);
		x = saturate(x*0.25 + 0.5);
		output.transColor = lerp(_MainColor.rgb, _TargetColor.rgb, x);
		output.worldPos = worldPos;
		//light & color
		fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
		fixed3 normalDir = normalize(mul(unity_ObjectToWorld, float4(v.normal, 0)).xyz);
		output.lightColor = _LightColor0.rgb * (dot(lightDir, normalDir) * 0.5 + 0.5) + UNITY_LIGHTMODEL_AMBIENT.rgb;
		fixed3 viewDir = normalize(float3((_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex)).xyz));
		fixed NdotV = dot(normalDir, viewDir);
		output.reflectDir.xyz = reflect(-viewDir, normalDir);
		output.reflectDir.w = _ReflectAmount * pow(1 - NdotV, _ReflFresnelPower) + _ReflFresnelOffset;
		output.rimMultiply = pow(1 - NdotV, _RimPower) * _RimColorMultiply;

		return output;
	}
	fixed4 frag(VertexOutput i) :COLOR
	{
		fixed4 c = fixed4(0,0,0,1);
	fixed3 cubeColor = texCUBE(_CubeMap, i.reflectDir.xyz).rgb * i.reflectDir.w;
	c.rgb = (i.transColor.rgb + cubeColor + i.transColor.rgb * i.rimMultiply) * i.lightColor;
	//final color
	c.rgb = c.rgb * saturate((i.worldPos.y + _OcclusionOffset) * _OcclusionStrength) * _FinalColorAdjust;
	return c;
	}
		ENDCG
		Pass
	{
		Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
#define UNITY_PASS_FORWARDBASE
#define multi_compile_fwdbase_fullshadows
			ENDCG
	}
	}
}