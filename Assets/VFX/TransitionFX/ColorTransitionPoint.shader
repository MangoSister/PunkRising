Shader "Custom/ColorTransitionPoint" 
{
	Properties 
	{
		_SourcePos ("Source Position", Vector) = (0,0,0,1)			
		_ThresDist ("Threshold Distance", float) = 1
		_SoftEdgeWidth("Soft Edge Width", Range(0,1)) = 0.1
		_InsideColor ("Inside Color", Color) = (1,1,1,1)
		_OutsideColor ("Outside Color", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input 
		{
			float2 uv_MainTex;
			float3 worldPos;			
		};

		half _Glossiness;
		half _Metallic;
		
		uniform half3 _SourcePos;
		uniform half _ThresDist;
		uniform fixed4 _InsideColor;
		uniform fixed4 _OutsideColor;
		uniform float _SoftEdgeWidth;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			float dist = distance(IN.worldPos, _SourcePos);
			// Albedo comes from a texture tinted by color
			fixed4 c = lerp(_InsideColor, _OutsideColor, saturate((dist - _ThresDist) / _SoftEdgeWidth));
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
