Shader "Custom/GrabPassTintShader" {

	Properties {
		_Tint ("Tint", Color) = (1,1,1,1)
	}

	SubShader {
		Tags {"Queue" = "Transparent"}
		GrabPass {"_GrabTex"}
		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			half4 _Tint;
			sampler2D _GrabTex;

			struct vertInput {
				float4 vertexPos : POSITION;
			};
			struct vertOutput {
				float4 vertexPos : POSITION;
				float4 grabUV : TEXCOORD1;
			};

			vertOutput vert (vertInput input) {
				vertOutput output;
				output.vertexPos = UnityObjectToClipPos(input.vertexPos);
				output.grabUV = ComputeGrabScreenPos(output.vertexPos);
				return output;
			}

			half4 frag (vertOutput input) : COLOR {
				fixed4 colour = tex2Dproj(_GrabTex, UNITY_PROJ_COORD(input.grabUV) + 2);
				colour *= _Tint;
				return colour; 
			}

			ENDCG
		}
	}

	Fallback "Diffuse"

}
