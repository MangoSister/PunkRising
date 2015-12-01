Shader "Custom/TexTransitionPoint" 
{
	Properties
	{
		_SourcePos("Source Position", Vector) = (0,0,0,1)
		_ThresDist("Threshold Distance", float) = 1
		_SoftEdgeWidth("Soft Edge Width", Range(0,1)) = 0.1
		_InsideTex("Inside Tex", 2D) = "white"{}
		_OutsideTex("Outside Tex", 2D) = "white"{}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

	struct Input
	{
		float2 uv_InsideTex;
		float3 worldPos;
	};

	half _Glossiness;
	half _Metallic;

	uniform half3 _SourcePos;
	uniform half _ThresDist;
	uniform sampler2D _InsideTex;
	uniform sampler2D _OutsideTex;
	uniform float _SoftEdgeWidth;

	void surf(Input IN, inout SurfaceOutputStandard o)
	{
		float dist = distance(IN.worldPos, _SourcePos);
		// Albedo comes from a texture tinted by color
		fixed4 insideTexCol = tex2D(_InsideTex, IN.uv_InsideTex);
		fixed4 outsideTexCol = tex2D(_OutsideTex, IN.uv_InsideTex);
		fixed4 c = lerp(insideTexCol, outsideTexCol, saturate((dist - _ThresDist) / _SoftEdgeWidth));
		o.Albedo = c.rgb;
		// Metallic and smoothness come from slider variables
		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		o.Alpha = c.a;
	}
	ENDCG
	}

		FallBack "Diffuse"
}
