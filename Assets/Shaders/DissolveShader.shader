Shader "Custom/Dissolve Shader" {

	Properties {
		[PerRendererData]_DissolveAmount ("Dissolve Amount", Range(0,1)) = 1
		[PerRendererData]_MainTex ("Texture", 2D) = "white"{}
		_NoiseTex ("Noise", 2D) = "white"{}
		_DissolveColour1 ("Dissolve Colour 1", Color) = (1,1,1,1)
		_DissolveColour2 ("Dissolve Colour 2", Color) = (1,1,1,1)
		_ColourThreshold1 ("Colour Threshold 1", Range(0,0.2)) = 0.1
		_ColourThreshold2 ("Colour Threshold 2", Range(0,0.2)) = 0.1
	}

	SubShader {
		Tags {"Queue" = "Transparent"}

		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float _DissolveAmount, _ColourThreshold1, _ColourThreshold2;
			sampler2D _MainTex, _NoiseTex;
			fixed4 _DissolveColour1, _DissolveColour2;

			struct vertInput {
				float4 pos : POSITION;
				float2 mainUV : TEXCOORD0;
			};

			struct vertOutput {
				float4 pos : SV_POSITION;
				float2 mainUV : TEXCOORD1;
			};

			vertOutput vert (vertInput input) {
				vertOutput output;
				output.pos = UnityObjectToClipPos(input.pos);
				output.mainUV = input.mainUV;
				return output;
			}

			half4 frag (vertOutput input) : COLOR {
				// noise is a value 0-1
				float noise = tex2Dlod (_NoiseTex, float4(input.mainUV, 0, 0));
				float threshold = noise - _DissolveAmount;
				clip(threshold);

				half4 colour = tex2Dlod (_MainTex, float4(input.mainUV, 0, 0));
				colour.a -= saturate(_DissolveAmount);

				float useDissolveColour1 = noise - _DissolveAmount < _ColourThreshold1;
				colour = (colour * (1 - useDissolveColour1)) + (_DissolveColour1 * useDissolveColour1);

				float useDissolveColour2 = noise - _DissolveAmount < _ColourThreshold2;
				colour = (colour * (1 - useDissolveColour2)) + (_DissolveColour2 * useDissolveColour2);

				return colour;
			}


			ENDCG
		}
	}

}