Shader "Custom/GuitarChargeShader" 
{
	Properties 
	{
		_PlanePos ("Source Plane Position", Vector) = (0,0,0,1)
		_PlaneNormal("Source Plane Normal", Vector) = (1,0,0,0)
		_ThresDist ("Threshold Distance", float) = 1
		_SoftEdgeWidth("Soft Edge Width", Range(0,1)) = 0.1

		_MainTex("Main Tex", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_EmissionSwitch("Glow Switch", Range(0,1)) = 0
		_EmissionInstensity("Glow Intensity", Range(0,10)) = 0.2
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#include "UnityCG.cginc"
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input 
		{
			float2 uv_MainTex;
			float3 localPos;
			float3 viewDir;
		};

		uniform half _Glossiness;
		uniform half _Metallic;
		uniform fixed _EmissionInstensity;
		uniform fixed _EmissionSwitch;

		uniform half3 _PlanePos;
		uniform fixed3 _PlaneNormal;
		uniform half _ThresDist;
		uniform sampler2D _MainTex;
		uniform float _SoftEdgeWidth;

		void vert(inout appdata_base v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.localPos = v.vertex.xyz;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			//compute in local space instead of world space

			float dist = dot(IN.localPos - _PlanePos, _PlaneNormal);
			fixed4 texCol = tex2D(_MainTex, IN.uv_MainTex);
			fixed grayScale = 0.21*texCol.r + 0.72*texCol.g + 0.07*texCol.b;
			fixed4 grayTexCol = fixed4(grayScale, grayScale, grayScale, 1);
			// Albedo comes from a texture tinted by color
			fixed4 c = lerp(texCol, grayTexCol, saturate((dist - _ThresDist)/_SoftEdgeWidth));
			o.Albedo = c.rgb;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			if(_EmissionSwitch > 0.5f)
				o.Emission = pow(rim, _EmissionInstensity) * c.rgb;
			else o.Emission = fixed3(0,0,0);
			o.Alpha = c.a;
		}
		ENDCG
	} 
	
	FallBack "Diffuse"
}
