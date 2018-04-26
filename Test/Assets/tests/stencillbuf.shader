// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/aaa" {
	Properties {
		_Color2 ("Color2", Color) = (1,1,1,1)
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader{
        Tags { "RenderType"="Transparent"
                "Queue" = "Transparent"
                "IgnoreProjector"="True" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha 
        ZWrite Off
        Cull off

        Pass
        {
            Stencil
            {
                Ref 1
                Comp NotEqual
				Pass replace
            }

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
			fixed4 _Color;
			fixed4 _Color2;

            fixed4 frag (v2f_img i) : SV_Target
            {
				fixed4 tex = tex2D(_MainTex, i.uv)* 0;
                return tex;
            }
            ENDCG
        }
	}
	FallBack "Diffuse"

}
