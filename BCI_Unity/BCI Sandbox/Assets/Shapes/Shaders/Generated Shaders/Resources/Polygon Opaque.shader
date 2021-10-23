Shader "Shapes/Polygon Opaque" {
	Properties {
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("Z Test", int) = 4
		_ZOffsetFactor ("Z Offset Factor", Float ) = 0
		_ZOffsetUnits ("Z Offset Units", int ) = 0
	}
	SubShader {
		Tags {
			"IgnoreProjector" = "True"
			"Queue" = "AlphaTest"
			"RenderType" = "TransparentCutout"
			"DisableBatching" = "True"
		}
		Pass {
			Cull Off
			ZTest [_ZTest]
			Offset [_ZOffsetFactor], [_ZOffsetUnits]
			AlphaToMask On
			HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_instancing
				#define OPAQUE
				#include "../../Core/Polygon Core.cginc"
			ENDHLSL
		}
		Pass {
			Name "Picking"
			Tags { "LightMode" = "Picking" }
			Cull Off
			HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_instancing
				#define OPAQUE
				#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap
				#define SCENE_VIEW_PICKING
				#include "../../Core/Polygon Core.cginc"
			ENDHLSL
		}
		Pass {
			Name "Selection"
			Tags { "LightMode" = "SceneSelectionPass" }
			Cull Off
			HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_instancing
				#define OPAQUE
				#pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap
				#define SCENE_VIEW_OUTLINE_MASK
				#include "../../Core/Polygon Core.cginc"
			ENDHLSL
		}
	}
}
