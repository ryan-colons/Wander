Shader "Custom/FireEffectShader" {

	Properties {
		_NoiseTex ("Noise", 2D) = "white"{}
		_DistortTex ("Distortion", 2D) = "white"{}
		_ShapeTex ("Shape", 2D) = "white"{}

		_Distortion ("Distortion Factor", Range(0, 1)) = 0.5
		_GradientColour1 ("Gradient 1", Color) = (1,1,1,1)
		_GradientColour2 ("Gradient 2", Color) = (0,0,0,0)
		_GradientTint1 ("Tint 1", Color) = (1,1,1,1)
		_GradientTint2 ("Tint 2", Color) = (0,0,0,0)

		_Height ("Height", Range(-4, 10)) = 1
		_HardCutoff ("Cutoff", Range(1, 40)) = 30
		_ScrollSpeed ("Scroll Speed", float) = 3
	}

	SubShader {
		Tags {"Queue" = "Geometry"}
		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _NoiseTex, _DistortTex, _ShapeTex;
			float4 _NoiseTex_ST, _DistortTex_ST, _ShapeTex_ST;
			float _Distortion;
			float4 _GradientColour1, _GradientColour2;
			float4 _GradientTint1, _GradientTint2;
			float _Height, _HardCutoff, _ScrollSpeed;

			struct vertInput {
				float4 pos : POSITION;
				float2 noiseUV : TEXCOORD0;
			};

			struct vertOutput {
				float4 pos : POSITION;
				float2 noiseUV : TEXCOORD1;
				float2 distortUV : TEXCOORD2;
				float2 shapeUV : TEXCOORD3;
			};

			vertOutput vert (vertInput input) {
				vertOutput output;
				output.pos = UnityObjectToClipPos(input.pos);
				output.noiseUV = TRANSFORM_TEX(input.noiseUV, _NoiseTex);
				output.distortUV = TRANSFORM_TEX(input.noiseUV, _DistortTex);
				output.shapeUV = TRANSFORM_TEX(input.noiseUV, _ShapeTex);
				return output;
			}

			fixed4 frag (vertOutput input) : COLOR {
				fixed4 distort = tex2D(_DistortTex, input.distortUV) * _Distortion;
				fixed2 distortedUV = fixed2(input.noiseUV.x - distort.g, input.noiseUV.y - distort.r - _Time.x * _ScrollSpeed);
				fixed4 noise = tex2D(_NoiseTex, distortedUV);

				float4 gradientColour = lerp(_GradientColour1, _GradientColour2, input.distortUV.y);
				float4 gradientTint = lerp(_GradientTint1, _GradientTint2, input.distortUV.y);
				float4 gradientFadeToTop = lerp(float4(2,2,2,2), float4(0,0,0,0), input.distortUV.y);

				fixed4 shapePoint = tex2D(_ShapeTex, input.shapeUV);

				//noise = 1 - (noise * _Height + (1 - shapePoint * _HardCutoff)); // shape mask
				noise += gradientFadeToTop;
				//noise = saturate(noise.a * _HardCutoff); // enforce max height
				//clip(noise);
				return noise * gradientColour * gradientTint;
			}

			ENDCG
		}
		
	}

}