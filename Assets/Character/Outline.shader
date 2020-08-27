// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Outline" {

	Properties{
		_Color("Main Color", Color) = (1, 1, 1, 1)
		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess("Shininess", Range(0.01, 1)) = 0.078125
		_Gloss("Gloss", Range(0.0, 2.0)) = 1
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_MainTex("Base Texture", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_GlossMap("Gloss Map", 2D) = "gloss" {}
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineWidth("Outline Width", Range(0, 0.5)) = 0.03
		_EmissionMap("Emission Map", 2D) = "black" {}
		_EmissionColor("Emission Color", Color) = (0,0,0)

	}

		Subshader{

		Tags{
		"RenderType" = "Opaque"
	}

		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows
		fixed4 _Color;
		half _Shininess;
		half _Gloss;
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _GlossMap;
		sampler2D _EmissionMap;
		half _Glossiness;
		half _Metallic;
		float4 _EmissionColor;
		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_GlossMap;
			float2 uv_EmissionMap;
		};
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Smoothness = tex2D(_GlossMap, IN.uv_GlossMap).a * _Gloss;
			o.Emission = tex2D(_EmissionMap, IN.uv_MainTex).rgb * _EmissionColor.rgb;
			//o.Specular = _Shininess;
		}
		void vert(inout appdata_full v)
		{
			// Transform the vertex coordinates from model space into world space
			float4 vv = mul(unity_ObjectToWorld, v.vertex);

			// Now adjust the coordinates to be relative to the camera position
			vv.xyz -= _WorldSpaceCameraPos.xyz;

			// Reduce the y coordinate (i.e. lower the "height") of each vertex based
			// on the square of the distance from the camera in the z axis, multiplied
			// by the chosen curvature factor
			vv = float4(0.0f, (vv.z * vv.z) * -_Curvature, 0.0f, 0.0f);

			// Now apply the offset back to the vertices in model space
			v.vertex += mul(unity_WorldToObject, vv);
		}


	ENDCG

		Pass{

		Cull Front

		CGPROGRAM

		#pragma vertex VertexProgram
		#pragma fragment FragmentProgram

		half _OutlineWidth;

float4 VertexProgram(
        float4 position : POSITION,
        float3 normal : NORMAL) : SV_POSITION {

    float4 clipPosition = UnityObjectToClipPos(position);
    float3 clipNormal = mul((float3x3) UNITY_MATRIX_VP, mul((float3x3) UNITY_MATRIX_M, normal));

    clipPosition.xyz += normalize(clipNormal) * _OutlineWidth;

    return clipPosition;

}

	half4 _OutlineColor;

	half4 FragmentProgram() : SV_TARGET{
		return _OutlineColor;
	}

		ENDCG

	}

	}

}