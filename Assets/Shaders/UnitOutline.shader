Shader "Custom/UnitOutline"
{
    Properties {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline width", Range (0.0, 1.0)) = .5
        _BehindColor ("Behind Color", Color) = (.8,.8,.8,1)

        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Metallic", 2D) = "white" {}
        _Metallic ("Smoothness", Range(0,1)) = 1.0 
        _Normal ("Normal", 2D) = "bump" {}
        _Occlusion ("Occlusion", 2D) = "white" {}
		_Emission ("Emission", 2D) = "black" {}
    }

 

	CGINCLUDE
	#include "UnityCG.cginc"

	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f {
		float4 pos : POSITION;
		float4 color : COLOR;
	};
	ENDCG



    SubShader {
        Tags { "Queue" = "Transparent" 
               "RenderType" = "Opaque"}

		//**************** Outline shader *****************//
        Pass {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Off
            ZWrite Off
            ZTest Greater
            Lighting On
 
            CGPROGRAM
            #pragma vertex outlineVert
            #pragma fragment frag

			uniform float _OutlineWidth;
			uniform float4 _OutlineColor;

			v2f outlineVert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				float2 offset = TransformViewToProjection(norm.xy);

				o.pos.xy += offset * o.pos.z * _OutlineWidth;
				o.color = _OutlineColor;
				return o;
			}

            half4 frag(v2f i) :COLOR {
                return i.color;
            }
            ENDCG
        }

		//**************** Behind shader *****************//
        Pass {
            Name "BEHIND"

            ZWrite Off
            ZTest Greater
            Lighting Off

            CGPROGRAM
            #pragma vertex behindVert
            #pragma fragment frag

			uniform float4 _BehindColor;

			v2f behindVert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = _BehindColor;
				return o;
			}

            half4 frag(v2f i) :COLOR {
                return i.color;
            }
            ENDCG
        }

        //**************** Standard shader *****************//
		ZWrite On
        ZTest LEqual
        Cull Off
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        
        #pragma target 4.0
        
        sampler2D _MainTex;
        sampler2D _Normal;
        sampler2D _Occlusion;
        sampler2D _Emission;
        
        struct Input 
        {
            float2 uv_MainTex;
        };
        
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        
        void surf (Input IN, inout SurfaceOutputStandard o) 
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            
            o.Occlusion = tex2D (_Occlusion, IN.uv_MainTex).r;
            o.Emission = tex2D (_Emission, IN.uv_MainTex);
            o.Normal = UnpackNormal(tex2D (_Normal, IN.uv_MainTex));
        }
        ENDCG
    }
    Fallback "Standard"
}
