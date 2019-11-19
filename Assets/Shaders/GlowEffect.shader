Shader "Custom/GlowEffect"
{
    Properties 
	{
		_ColorTint("Color Tint", Color) = (1, 1, 1, 1)
		_AlphaCutout("Alpha Cutout", Range(0,1)) = 0.5
		_MainTex("Albedo (RGBA)", 2D) = "white" {}
		_Glossiness ("Metallic", 2D) = "white" {}
        _Metallic ("Smoothness", Range(0,1)) = 1.0 
        _Normal ("Normal", 2D) = "bump" {}
        _Occlusion ("Occlusion", 2D) = "white" {}
		_EmissionMap ("Emission", 2D) = "black" {}
		_EmissionColor("Emission Color", Color) = (1, 1, 1, 1)

		_RimColor("Rim Color", Color) = (1, 1, 1, 1)
		_RimPower("Rim Power", Range(1.0, 6.0)) = 3.0

		_OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
		_OutlineWidth("Outline Width", Range(0.0, 2.0)) = 0.1
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};

	uniform float _OutlineWidth;
	uniform float4 _OutlineColor;
	uniform float _AlphaCutout;
	ENDCG

	SubShader 
	{
		Tags{ "Queue" = "Transparent-1" "IgnoreProjector" = "True" }

		//Outline shader
		Pass 
		{
			ZWrite Off
			Cull Back
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				appdata original = v;
				v.vertex.xyz += _OutlineWidth * normalize(v.vertex.xyz);

				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;

			}

			half4 frag(v2f i) : COLOR
			{
				half4 c = tex2D (_MainTex, i.uv);
				clip(c.a - _AlphaCutout);
				return _OutlineColor;
			}

			ENDCG
		}
		
		// Standard Shader
		Tags
		{
			"RenderType"="Transparent"
			"Queue" = "Geometry"
		}

		AlphaTest Greater 0.5

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows 
		#pragma target 4.0

		#include "UnityCG.cginc"

		struct Input {
			float4 color : Color;
			float2 uv_MainTex;
			float2 uv_Occlusion;
			float2 uv_Normal;
			float2 uv_EmissionMap;
			float3 viewDir;
		};

		float4 _ColorTint;
		sampler2D _MainTex;
		sampler2D _Normal;
		sampler2D _Occlusion;
		sampler2D _EmissionMap;
		half _Glossiness;
        half _Metallic;
		float4 _RimColor;
		float _RimPower;


		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _ColorTint;
			clip(c.a - _AlphaCutout);
			
			o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
			o.Occlusion = tex2D (_Occlusion, IN.uv_Occlusion).r;
			o.Normal = UnpackNormal(tex2D(_Normal,IN.uv_Normal));
			o.Emission = tex2D(_EmissionMap, IN.uv_EmissionMap);

			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Albedo.rgb += _RimColor.rgb * pow(rim, _RimPower);
		}
		ENDCG
	} 
	FallBack "Standard"
}
