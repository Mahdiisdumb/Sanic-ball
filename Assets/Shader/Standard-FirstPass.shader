Shader "Nature/Terrain/Standard" {
Properties {
[HideInInspector]  _Control ("Control (RGBA)", 2D) = "red" { }
[HideInInspector]  _Splat3 ("Layer 3 (A)", 2D) = "white" { }
[HideInInspector]  _Splat2 ("Layer 2 (B)", 2D) = "white" { }
[HideInInspector]  _Splat1 ("Layer 1 (G)", 2D) = "white" { }
[HideInInspector]  _Splat0 ("Layer 0 (R)", 2D) = "white" { }
[HideInInspector]  _Normal3 ("Normal 3 (A)", 2D) = "bump" { }
[HideInInspector]  _Normal2 ("Normal 2 (B)", 2D) = "bump" { }
[HideInInspector]  _Normal1 ("Normal 1 (G)", 2D) = "bump" { }
[HideInInspector]  _Normal0 ("Normal 0 (R)", 2D) = "bump" { }
[HideInInspector] [Gamma]  _Metallic0 ("Metallic 0", Range(0,1)) = 0
[HideInInspector] [Gamma]  _Metallic1 ("Metallic 1", Range(0,1)) = 0
[HideInInspector] [Gamma]  _Metallic2 ("Metallic 2", Range(0,1)) = 0
[HideInInspector] [Gamma]  _Metallic3 ("Metallic 3", Range(0,1)) = 0
[HideInInspector]  _Smoothness0 ("Smoothness 0", Range(0,1)) = 1
[HideInInspector]  _Smoothness1 ("Smoothness 1", Range(0,1)) = 1
[HideInInspector]  _Smoothness2 ("Smoothness 2", Range(0,1)) = 1
[HideInInspector]  _Smoothness3 ("Smoothness 3", Range(0,1)) = 1
[HideInInspector]  _MainTex ("BaseMap (RGB)", 2D) = "white" { }
[HideInInspector]  _Color ("Main Color", Color) = (1,1,1,1)
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Lambert
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutput o)
		{
			float4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}