Shader "Custom/SculptedHair" {
	Properties {
         _Color ("Main Color", Color) = (1,1,1,1)
         _HighlightColor ("Highlight Color", Color) = (1,1,1,1)
         _HighlightColor2 ("Highlight Color", Color) = (1,1,1,1)
         _MainTex ("Diffuse (RGB) Alpha (A)", 2D) = "white" {}
         _MetallicSmooth ("Metallic (RGB) _Smooth (A)", 2D) = "white" {}
         _BumpMap ("Normal (Normal)", 2D) = "bump" {}
         _AnisoTex ("Anisotropic Direction (Normal)", 2D) = "bump" {}
         _AnisoOffset ("Anisotropic Highlight Offset", Range(-1,1)) = -0.2
          _AnisoOffset2 ("Anisotropic Highlight Offset", Range(-1,1)) = -0.2
		 _Gloss ("Gloss", Range(0,1)) = 0.2
		 _Gloss2 ("Gloss2", Range(0,1)) = 0.2
		 _Specularity ("Specularity", Range(0,1)) = 0.2
		 _Specularity2 ("Specularity2", Range(0,1)) = 0.2
		 _Reflection ("Reflection", Range(0,1)) = 0.2
		 _value ("value", Float) = 0.0
     }

     SubShader{
         Tags { "RenderType" = "Opaque" }
     
         CGPROGRAM
		 	#pragma surface surf Aniso fullforwardshadows addshadow
            #pragma target 3.0

			struct Input
			{
				float2 uv_MainTex;
				float2 uv_AnisoTex;
			};

			struct SurfaceOutputAniso {
				fixed3 Albedo;
				fixed3 Ramp;
				fixed3 Emission;
				fixed3 Normal;
				fixed4 AnisoDir;
				fixed Alpha;
				fixed Smoothness;
			};

			fixed4 _Color;
			fixed4 _HighlightColor;
			fixed4 _HighlightColor2;
			fixed _AnisoOffset;
			fixed _AnisoOffset2;
			float _value;
			fixed _Gloss, _Gloss2, _Specularity, _Specularity2, _Reflection;

			sampler2D _MainTex, _SpecularTex, _BumpMap, _AnisoTex, _MetallicSmooth;
			
			inline fixed4 LightingAniso (SurfaceOutputAniso s, fixed3 lightDir, fixed3 viewDir, fixed atten)
			{
				fixed3 h = normalize(normalize(lightDir) + normalize(viewDir));
				float NdotL = saturate(dot(s.Normal, lightDir));

				fixed HdotA = dot(normalize(s.Normal + s.AnisoDir.rgb), h);
				float aniso = max(0, sin(radians((HdotA + _AnisoOffset + (s.Smoothness*0.1)) * 180 )));
				float aniso2 = max(0, sin(radians((HdotA + _AnisoOffset2 + (s.Smoothness*0.1)) * 180 )));

				float spec = saturate(dot(s.Normal, h));
				float spec2 = spec;
				
				spec = saturate(pow(lerp(spec, aniso, s.AnisoDir.a), _Gloss  * 128) * _Specularity);
				spec2 = saturate(pow(lerp(spec2, aniso2, s.AnisoDir.a), _Gloss2 * 128) * _Specularity2);

				
				float4 refColor = 1.0;
				float3 refDir = reflect(-viewDir, s.Normal);
				float4 reflection = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, refDir);
				refColor.rgb = DecodeHDR(reflection, unity_SpecCube0_HDR);
				refColor.a = 1.0;

				fixed4 c;
				c.rgb = ((s.Albedo * _LightColor0.rgb * NdotL) + (lerp(_HighlightColor.rgb, refColor.rgb, _Reflection) * spec * NdotL)) * (atten * 2);
				c.rgb += lerp(_HighlightColor2.rgb, refColor.rgb, _Reflection) * spec2 * NdotL;
				c.a = 1;
				return c;
			}             
                 
			void surf (Input IN, inout SurfaceOutputAniso o)
			{
				fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Smoothness = tex2D(_MetallicSmooth, IN.uv_MainTex).a;
				o.Albedo = albedo.rgb;
				o.Alpha = albedo.a;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
				o.AnisoDir = fixed4(tex2D(_AnisoTex, IN.uv_AnisoTex).rgb, 1);
			}
         ENDCG
     }
     FallBack "Diffuse"
}
