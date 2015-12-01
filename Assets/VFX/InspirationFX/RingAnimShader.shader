Shader "Custom/RingAnimShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_RingMask("Ring Mask", 2D) = "white" {}
		_RingMaskScale("Ring Mask Scale", Range(0,1)) = 0
		_FadeOut("FadeOut", Range(0,1)) = 0

	}
	SubShader {
		Tags 
		{ 
			"Queue"="Transparent"
			"RenderType"="Transparent" 
		}
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows keepalpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		
		
		
		struct Input 
		{
			float2 uv_MainTex;
		};

		uniform sampler2D _MainTex;
		uniform sampler2D _RingMask;

		uniform fixed _RingMaskScale;
		uniform half _Glossiness;
		uniform half _Metallic;
		uniform fixed4 _Color;
		uniform fixed _FadeOut;
		
		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			float2 scaledUv = (IN.uv_MainTex - fixed2(0.5 - 0.5*_RingMaskScale, 0.5 - 0.5*_RingMaskScale)) / _RingMaskScale;
			fixed4 mask = tex2D(_RingMask, saturate(scaledUv));
			if (scaledUv.x < 0 || scaledUv.x > 1 || scaledUv.y < 0 || scaledUv.y > 1)
				mask = fixed4(0, 0, 0, 1);

			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = mask.r * (1- _FadeOut);
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
