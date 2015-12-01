Shader "Custom/TexLocalTransitionPlane" 
{
	Properties
	{
		_PlanePos("Source Plane Position", Vector) = (0,0,0,1)
		_PlaneNormal("Source Plane Normal", Vector) = (1,0,0,0)
		_ThresDist("Threshold Distance", float) = 1
		_SoftEdgeWidth("Soft Edge Width", Range(0,1)) = 0.1

		_BeforeTex("Before Tex (RGB)", 2D) = "white" {}
		_AfterTex("After Tex (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}

	SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#include "UnityCG.cginc"
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows vertex:vert

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			struct Input 
			{
				float2 uv_BeforeTex;
				float3 localPos;
			};

			uniform sampler2D _BeforeTex;
			uniform sampler2D _AfterTex;
			uniform half3 _PlanePos;
			uniform fixed3 _PlaneNormal;
			uniform half _ThresDist;
			uniform float _SoftEdgeWidth;

			half _Glossiness;
			half _Metallic;

			void vert(inout appdata_base v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.localPos = v.vertex.xyz;
			}

			void surf(Input IN, inout SurfaceOutputStandard o) 
			{
				//compute in local space instead of world space
				float dist = dot(IN.localPos - _PlanePos, _PlaneNormal);

				fixed4 beforeTexCol = tex2D(_BeforeTex, IN.uv_BeforeTex);
				fixed4 afterTexCol = tex2D(_AfterTex, IN.uv_BeforeTex);
				fixed4 c = lerp(beforeTexCol, afterTexCol, saturate((dist - _ThresDist) / _SoftEdgeWidth));
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
