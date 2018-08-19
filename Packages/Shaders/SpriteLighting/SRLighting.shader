// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SRLighting" {

	Properties{
		_MainTex ("Base RGBA", 2D) = "white" {}
		_NormalTex ("Normalmap", 2D) = "bump" {}
		_Color ("Diffuse Material Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_SpecularColor ("Specular Material Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Shininess ("Shininess", Float) = 5
		_CelShadingLevels ("Cel Shading Levels", Float) = 0
	}

		SubShader{
		// these are applied to all of the Passes in this SubShader
		ZWrite Off
		ZTest Always
		Fog{ Mode Off }
		Lighting On
		Cull Off

		// -------------------------------------
		// Base pass:
		// -------------------------------------
		Pass{

		Tags{ "LightMode" = "ForwardBase" "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM

#pragma vertex vert  
#pragma fragment frag 

#include "UnityCG.cginc"

		uniform sampler2D _MainTex;

	struct VertexInput {

		float4 vertex : POSITION;
		float4 color : COLOR;
		float4 uv : TEXCOORD0;
	};

	struct VertexOutput {

		float4 pos : POSITION;
		float4 color : COLOR;
		float2 uv : TEXCOORD0;
	};

	VertexOutput vert (VertexInput i) {

		VertexOutput o;

		o.pos = UnityObjectToClipPos (i.vertex);
		o.color = i.color;
		o.uv = i.uv;

		return o;
	}

	float4 frag (VertexOutput i) : COLOR{

		float4 diffuseColor = tex2D (_MainTex, i.uv);
		float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.xyz * diffuseColor.xyz * i.color.xyz;

		return float4(ambientLighting, diffuseColor.a);
	}

		ENDCG
	}

		// -------------------------------------
		// Lighting Pass: Lights must be set to Important
		// -------------------------------------
		Pass{

		Tags{ "LightMode" = "ForwardAdd" "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend One One // additive blending 

		CGPROGRAM

#pragma vertex vert  
#pragma fragment frag 

#include "UnityCG.cginc"

		// shader uniforms
		uniform sampler2D _MainTex;   // source diffuse texture
		uniform sampler2D _NormalTex; // normal map lighting texture (set to import type: Lightmap)
		uniform float4 _LightColor0;  // color of light source 
		uniform float4 _SpecularColor;
		uniform float _Shininess;
		uniform float _CelShadingLevels;

	struct vertexInput {
		float4 vertex : POSITION;
		float4 color : COLOR;
		float4 uv : TEXCOORD0;
	};

	struct fragmentInput {
		float4 pos : SV_POSITION;
		float4 color : COLOR0;
		float2 uv : TEXCOORD0;
		float4 posWorld : TEXCOORD1; // change this to distance to light and pass from vert to frag
	};

	// -------------------------------------
	fragmentInput vert (vertexInput i) {

		fragmentInput o;

		o.pos = UnityObjectToClipPos (i.vertex);
		o.posWorld = mul (unity_ObjectToWorld, i.vertex);

		o.uv = i.uv;
		o.color = i.color;

		return o;
	}

	// -------------------------------------
	float4 frag (fragmentInput i) : COLOR{

		// get value from normal map and sub 0.5 and mul by 2 to change RGB range 0..1 to normal range -1..1
		float3 normalDirection = (tex2D (_NormalTex, i.uv).xyz - 0.5f) * 2.0f;

		// mul by world to object matrix, which handles rotation, etc
		normalDirection = mul (float4(normalDirection, 0.5f), unity_WorldToObject).xyz;

		// negate Z so that lighting works as expected (sprites further away from the camera than a light are lit, etc.)
		normalDirection.z *= -1;

		// normalize direction
		normalDirection = normalize (normalDirection);

		// dist to point light
		float3 vertexToLightSource = _WorldSpaceLightPos0.xyz -i.posWorld;
		float3 distance = length (vertexToLightSource);

		// calc attenuation
		float attenuation = 1.0 / distance;
		float3 lightDirection = normalize (vertexToLightSource);

		// calc diffuse lighting
		float normalDotLight = dot (normalDirection, lightDirection);
		float diffuseLevel = attenuation * max (0.0, normalDotLight);

		// calc specular ligthing
		float specularLevel = 0.0;
		// make sure the light is on the proper side
		if (normalDotLight > 0.0) {

			// since orthographic
			float3 viewDirection = float3(0.0, 0.0, -1.0);
			specularLevel = attenuation * pow (max (0.0, dot (reflect (-lightDirection, normalDirection), viewDirection)), _Shininess);
		}

		// Add cel-shading if enough levels were specified
		if (_CelShadingLevels >= 2)
		{
			diffuseLevel = floor (diffuseLevel * _CelShadingLevels) / (_CelShadingLevels - 0.5f);
			specularLevel = floor (specularLevel * _CelShadingLevels) / (_CelShadingLevels - 0.5f);
		}

		// calc color components
		float4 diffuseColor = tex2D (_MainTex, i.uv);
		float3 diffuseReflection = diffuseColor.xyz * diffuseLevel * i.color * _LightColor0.xyz;
		float3 specularReflection = _SpecularColor.xyz * specularLevel * i.color * _LightColor0.xyz;

		// use the alpha from diffuse. mul by diffuseColor.a to resolve issues with transparency on overlapping sprites in same FRenderLayer
		return diffuseColor.a * float4(diffuseReflection + specularReflection, diffuseColor.a);
	}

		ENDCG
	} // end Pass
	  // -------------------------------------
	  // -------------------------------------

	} // end SubShader

	  // fallback shader - comment out during dev
	  // Fallback "Diffuse"
}