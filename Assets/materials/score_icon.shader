// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/score_icon" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGBA)", 2D) = "white" {}
		_MaskTex ("Mask (RGBA)", 2D) = "white" {}
		_WrapX ("WrapX (float)", float) = 0
		_WrapY ("WrapY (float)", float) = 0
	}
	SubShader {
		Tags {
			"Queue" = "Transparent" 
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest Off
		ZWrite Off
		Cull Off
		Lighting Off

		Pass {
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			sampler2D _MaskTex;
			fixed4 _Color;
			float _WrapX;
			float _WrapY;

			struct a_base {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			a_base vert(float4 vertex : POSITION, float2 texcoord : TEXCOORD0) {
			    a_base o;
			    o.vertex = UnityObjectToClipPos(vertex);    
			    o.texcoord = texcoord;
			    return o;
			}

			fixed4 frag(a_base i) : COLOR {
				float2 uv = i.texcoord;
				uv.x *= 0.4f;
				uv.x += _WrapX;
				uv.y *= 0.4f;
				uv.y += -_WrapY * 0.5 - 0.45;
				fixed4 c = tex2D(_MainTex, uv) * _Color;
				c.a *= tex2D(_MaskTex, i.texcoord).a;
				return c;
			}

		ENDCG
		} 
	}

	FallBack Off
}