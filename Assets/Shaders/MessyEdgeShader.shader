Shader "Custom/MessyEdgeShader" {

	Properties {
		_MainTex ("Texture", 2D) = "white"{}
		_NoiseTex ("Edge Noise", 2D) = "white"{}
	}

	SubShader {
		Tags {}

		CGPROGRAM

		#pragma surface surf Lambert vertex:vertFunc

		sampler2D _MainTex;
		sampler2D _NoiseTex;

		struct Input{
			float2 uv_MainTex;
			float2 uv_NoiseTex;
			float3 vertBasedColour;
		};

		void vertFunc (inout appdata_full vert, out Input output) {
			UNITY_INITIALIZE_OUTPUT(Input, output);
			float4 noise = tex2Dlod(_NoiseTex, float4(output.uv_NoiseTex.xy, 0, 0));
			vert.vertex.x += noise.r * 1.0;
			vert.vertex.y += noise.g * 2.0;
			vert.vertex.z += noise.b * 3.0;
			output.vertBasedColour = vert.vertex;
		}

		void surf (Input IN, inout SurfaceOutput output) {
			output.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			output.Albedo *= IN.vertBasedColour;
		}

		ENDCG
	}

	Fallback "Diffuse"

}