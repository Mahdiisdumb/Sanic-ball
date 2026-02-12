Shader "Nature/Terrain/Specular" {
Properties {
 _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
 _Shininess ("Shininess", Range(0.03,1)) = 0.078125
[HideInInspector]  _Control ("Control (RGBA)", 2D) = "red" { }
[HideInInspector]  _Splat3 ("Layer 3 (A)", 2D) = "white" { }
[HideInInspector]  _Splat2 ("Layer 2 (B)", 2D) = "white" { }
[HideInInspector]  _Splat1 ("Layer 1 (G)", 2D) = "white" { }
[HideInInspector]  _Splat0 ("Layer 0 (R)", 2D) = "white" { }
[HideInInspector]  _Normal3 ("Normal 3 (A)", 2D) = "bump" { }
[HideInInspector]  _Normal2 ("Normal 2 (B)", 2D) = "bump" { }
[HideInInspector]  _Normal1 ("Normal 1 (G)", 2D) = "bump" { }
[HideInInspector]  _Normal0 ("Normal 0 (R)", 2D) = "bump" { }
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