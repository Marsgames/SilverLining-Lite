Shader "Custom/Transparency"
{
    Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGBA)", 2D) = "white" {}
		_Alpha ("Alpha", Range (0.0, 1.0)) = .5
    }



    SubShader 
	{
		Tags 
		{ 
			"Queue" = "Transparent"
			"RenderType"="Transparent"
		}

		Zwrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Lambert alpha
        
        #pragma target 4.0

		#include "UnityCG.cginc"
        
        sampler2D _MainTex;
        
        struct Input 
        {
            float2 uv_MainTex;
        };
        
        fixed4 _Color;
		float _Alpha;        
        void surf (Input IN, inout SurfaceOutput o) 
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = _Alpha;
        }
        ENDCG
    }
    Fallback "Standard"
}
