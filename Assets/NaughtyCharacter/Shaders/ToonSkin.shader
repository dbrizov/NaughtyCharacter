// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/ToonSkin" {

	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_InternalColor("Internal Color", Color) = (1,1,1,1)
		_SpecularColor("Specular Color", Color) = (1,1,1,1)
		[Space]
	_MainTex("Albedo (RGB) SSS (A)", 2D) = "white" {}
	_SpecGloss("Specular (RGB) Glossiness (A)", 2D) = "white" {}
	_BumpMap("Bumpmap", 2D) = "bump" {}
	_DetailBumpMap("Detail Bumpmap", 2D) = "bump" {}
	[Space]
	_SSS("SSS Intensity", Range(0,1)) = 1
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Specular("Specular", Range(0,2)) = 0.0
	}

		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM

#include "UnityPBSLighting.cginc"

#pragma surface surf SimpleSSS fullforwardshadows
#pragma target 3.0

		sampler2D _MainTex;
	sampler2D _SpecGloss;
	sampler2D _BumpMap;
	sampler2D _DetailBumpMap;

	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
		float2 uv_DetailBumpMap;
	};

	half _Glossiness;
	half _Specular;
	fixed4 _Color, _InternalColor, _SpecularColor;
	half _SSS;


	// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
	// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
	// #pragma instancing_options assumeuniformscaling
	UNITY_INSTANCING_BUFFER_START(Props)

		UNITY_INSTANCING_BUFFER_END(Props)

		float3 SubsurfaceShadingSimple(float3 diffColor, float3 normal, float3 viewDir, float3 thickness, float3 lightDir, float3 lightColor)
	{
		half3 vLTLight = lightDir + normal * 1;
		half  fLTDot = pow(saturate(dot(viewDir, -vLTLight)), 3.5) * 1.5;
		half3 fLT = 1 * (fLTDot + 1.2) * (thickness);
		return diffColor * ((lightColor * fLT) * 0.4);
	}


	half4 LightingSimpleSSS(SurfaceOutputStandardSpecular s, half3 viewDir, UnityGI gi)
	{
		s.Normal = normalize(s.Normal);

		// energy conservation
		half oneMinusReflectivity;
		s.Albedo = EnergyConservationBetweenDiffuseAndSpecular(s.Albedo, s.Specular, /*out*/ oneMinusReflectivity);

		// shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
		// this is necessary to handle transparency in physically correct way - only diffuse component gets affected by alpha
		half outputAlpha;
		s.Albedo = PreMultiplyAlpha(s.Albedo, 1.0f, oneMinusReflectivity, /*out*/ outputAlpha);

		half4 c = UNITY_BRDF_PBS(s.Albedo, s.Specular, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);

		c.rgb += SubsurfaceShadingSimple(_InternalColor, s.Normal, viewDir, s.Alpha*_SSS, gi.light.dir, gi.light.color);

		c.a = outputAlpha;
		return c;
	}

	inline void LightingSimpleSSS_GI(
		SurfaceOutputStandardSpecular s,
		UnityGIInput data,
		inout UnityGI gi)
	{
#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
		gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
#else
		Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(s.Smoothness, data.worldViewDir, s.Normal, s.Specular);
		gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal, g);
#endif
	}


	void surf(Input IN, inout SurfaceOutputStandardSpecular o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		fixed4 s = tex2D(_SpecGloss, IN.uv_MainTex);
		o.Albedo = c.rgb;
		o.Normal = BlendNormals(UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)), UnpackNormal(tex2D(_DetailBumpMap, IN.uv_DetailBumpMap)));
		o.Specular = _Specular * s.rgb * _SpecularColor.rgb;
		o.Smoothness = _Glossiness * s.a;
		o.Alpha = c.a;
		//o.Emission = SubsurfaceShadingSimple(_InternalColor, o.Normal, IN.viewDir, c.a*_SSS, IN.lightDir, _LightColor0);
	}
	ENDCG
	}
		FallBack "Standard"
}
