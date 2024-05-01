//
// See notes in 'Shaded Unlit' shader
// This is the same, but supports basic blended transparency
//
Shader "Technie/Virtual Console/Shaded Unlit (Transparent)"
{
	Properties
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform fixed4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = normalize(mul(unity_ObjectToWorld, float4(v.normal.xyz, 0.0)).xyz);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				float3 lightDir = normalize(float3(0.2, 0.6, 0.2));
				float intensity = saturate(dot(i.normal, lightDir));
				intensity = saturate(intensity + 0.7);
				fixed4 col = _Color * float4(intensity, intensity, intensity, 1);
				return col;
			}
		ENDCG
		}
	}
}
