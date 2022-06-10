// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Simuranstudio/Particles/Opacity"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_Mask_1("Mask_1", 2D) = "white" {}
		_Mask_2("Mask_2", 2D) = "white" {}
		_Emission("Emission", Float) = 2
		_Vector0("Vector 0", Vector) = (0,0,0,0)
		_Vector1("Vector 1", Vector) = (0,0,0,0)
		_Mask_1Power("Mask_1 Power", Float) = 0
		[HDR]_Color0("Color 0", Color) = (0.5019608,0.5019608,0.5019608,1)
		_Tiling("Tiling", Vector) = (1,1,0,0)

	}


	Category 
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend One One
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {
			
				CGPROGRAM
				
				#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
				#endif
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "UnityShaderVariables.cginc"
				#define ASE_NEEDS_FRAG_COLOR


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					
				};
				
				
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform sampler2D _TextureSample0;
				uniform float4 _Vector1;
				uniform float2 _Tiling;
				uniform sampler2D _Mask_1;
				uniform float4 _Vector0;
				uniform float _Mask_1Power;
				uniform sampler2D _Mask_2;
				uniform sampler2D _Noise;
				uniform float4 _Color0;
				SamplerState sampler_TextureSample0;
				SamplerState sampler_Noise;
				uniform float _Emission;


				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID( i );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( i );

					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						i.color.a *= fade;
					#endif

					float2 texCoord2 = i.texcoord.xy * _Tiling + float2( 0,0 );
					float2 panner20 = ( 1.0 * _Time.y * (_Vector1).xy + texCoord2);
					float2 panner9 = ( 1.0 * _Time.y * (_Vector0).xy + texCoord2);
					float2 panner8 = ( 1.0 * _Time.y * (_Vector0).zw + float2( 0,0 ));
					float4 tex2DNode26 = tex2D( _TextureSample0, ( panner20 - ( ( (tex2D( _Mask_1, panner9 )).rg * _Mask_1Power ) * ( (tex2D( _Mask_2, panner8 )).rg * 0.0 ) ) ) );
					float2 panner24 = ( 1.0 * _Time.y * (_Vector1).zw + texCoord2);
					float4 tex2DNode28 = tex2D( _Noise, panner24 );
					

					fixed4 col = ( ( tex2DNode26 * tex2DNode28 * _Color0 * i.color * tex2DNode26.a * tex2DNode28.a * _Color0.a * i.color.a ) * _Emission );
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18500
7;7;1906;1004;4489.321;612.0571;1;True;False
Node;AmplifyShaderEditor.Vector2Node;75;-3895.321,-243.0571;Inherit;False;Property;_Tiling;Tiling;9;0;Create;True;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;1;-3336.492,95.1413;Inherit;False;1767.788;577.9193;Texture Mask1/Mask2 + Speed;16;21;18;17;15;12;11;9;8;7;6;5;4;3;69;70;71;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-3576.03,-264.8396;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;4;-3242.863,294.9313;Inherit;False;Property;_Vector0;Vector 0;5;0;Create;True;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;3;-3247.389,141.0886;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;7;-2980.985,248.3659;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;5;-3019.235,199.4124;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;6;-2975.797,365.1157;Inherit;False;False;False;True;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;9;-2687.903,201.129;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;8;-2691.185,404.3899;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;12;-2501.644,172.5759;Inherit;True;Property;_Mask_1;Mask_1;2;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;10;-2853.878,-509.8708;Inherit;False;1037.896;533.6285;Textures Speed;7;24;22;20;19;16;14;13;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;11;-2477.534,392.8275;Inherit;True;Property;_Mask_2;Mask_2;3;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;71;-2133.453,431.4857;Inherit;False;Constant;_Mask_2Power;Mask_2 Power;11;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-2174.401,269.733;Inherit;False;Property;_Mask_1Power;Mask_1 Power;7;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;14;-2819.84,-404.9511;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;69;-2137.453,351.4857;Inherit;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;13;-2674.274,-318.4672;Inherit;False;Property;_Vector1;Vector 1;6;0;Create;True;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;17;-2196.787,191.2131;Inherit;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-1869.453,324.4857;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;16;-2379.852,-317.2331;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1917.919,175.6178;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;19;-2128.158,-411.9194;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;20;-2073.211,-316.9446;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-1741.263,202.0731;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;22;-2362.074,-202.5214;Inherit;False;False;False;True;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;23;-1652.809,-321.5544;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;72;-1496.806,-605.3474;Inherit;False;408.0071;961.6622;Textures;4;26;28;45;25;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode;24;-2058.913,-142.3562;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;28;-1471.869,-323.3763;Inherit;True;Property;_Noise;Noise;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;25;-1401.376,128.6236;Inherit;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;45;-1417.386,-84.77975;Inherit;False;Property;_Color0;Color 0;8;1;[HDR];Create;True;0;0;False;0;False;0.5019608,0.5019608,0.5019608,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;26;-1447.464,-543.1437;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-654.9349,-292.3611;Inherit;True;8;8;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-417.9411,-190.7464;Float;False;Property;_Emission;Emission;4;0;Create;True;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-248.3778,-316.8318;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;-61.18317,-307.6994;Float;False;True;-1;2;ASEMaterialInspector;0;7;Simuranstudio/Particles/Opacity;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;4;1;False;-1;1;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;False;False;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;2;0;75;0
WireConnection;3;0;2;0
WireConnection;7;0;4;0
WireConnection;5;0;3;0
WireConnection;6;0;4;0
WireConnection;9;0;5;0
WireConnection;9;2;7;0
WireConnection;8;2;6;0
WireConnection;12;1;9;0
WireConnection;11;1;8;0
WireConnection;14;0;2;0
WireConnection;69;0;11;0
WireConnection;17;0;12;0
WireConnection;70;0;69;0
WireConnection;70;1;71;0
WireConnection;16;0;13;0
WireConnection;15;0;17;0
WireConnection;15;1;18;0
WireConnection;19;0;14;0
WireConnection;20;0;19;0
WireConnection;20;2;16;0
WireConnection;21;0;15;0
WireConnection;21;1;70;0
WireConnection;22;0;13;0
WireConnection;23;0;20;0
WireConnection;23;1;21;0
WireConnection;24;0;2;0
WireConnection;24;2;22;0
WireConnection;28;1;24;0
WireConnection;26;1;23;0
WireConnection;33;0;26;0
WireConnection;33;1;28;0
WireConnection;33;2;45;0
WireConnection;33;3;25;0
WireConnection;33;4;26;4
WireConnection;33;5;28;4
WireConnection;33;6;45;4
WireConnection;33;7;25;4
WireConnection;37;0;33;0
WireConnection;37;1;35;0
WireConnection;0;0;37;0
ASEEND*/
//CHKSM=EE44249A480E7E5659D0AC29B263C7D134DB4445