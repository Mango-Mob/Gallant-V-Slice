// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Simuranstudio/Particles/Black"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_MainTex3("MainTex", 2D) = "white" {}
		_Noise3("Noise", 2D) = "white" {}
		_Mask_1("Mask_1", 2D) = "white" {}
		_Mask_2("Mask_2", 2D) = "white" {}
		_SpeedMask1Mask2("Speed Mask1/Mask2", Vector) = (0,0,0,0)
		_Emission3("Emission", Float) = 2
		[HDR]_Color3("Color", Color) = (0.5,0.5,0.5,1)
		_Opacity3("Opacity", Range( 0 , 1)) = 1
		_SpeedMainNoise("Speed Main/Noise", Vector) = (0,0,0,0)
		_DisplayingMask("Displaying Mask", Float) = 0

	}


	Category 
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha OneMinusSrcAlpha
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
				uniform sampler2D _MainTex3;
				uniform float4 _SpeedMainNoise;
				uniform sampler2D _Mask_1;
				uniform float4 _SpeedMask1Mask2;
				uniform sampler2D _Mask_2;
				uniform float _DisplayingMask;
				uniform sampler2D _Noise3;
				uniform float4 _Color3;
				uniform float _Emission3;
				SamplerState sampler_MainTex3;
				SamplerState sampler_Noise3;
				uniform float _Opacity3;


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

					float2 texCoord10 = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float2 appendResult13 = (float2(texCoord10.x , texCoord10.y));
					float2 panner24 = ( 1.0 * _Time.y * (_SpeedMainNoise).xy + appendResult13);
					float2 panner15 = ( 1.0 * _Time.y * (_SpeedMask1Mask2).xy + appendResult13);
					float2 panner14 = ( 1.0 * _Time.y * (_SpeedMask1Mask2).zw + appendResult13);
					float4 tex2DNode31 = tex2D( _MainTex3, ( panner24 - ( (( tex2D( _Mask_1, panner15 ) * tex2D( _Mask_2, panner14 ) )).rg * _DisplayingMask ) ) );
					float2 panner29 = ( 1.0 * _Time.y * (_SpeedMainNoise).zw + appendResult13);
					float4 tex2DNode35 = tex2D( _Noise3, panner29 );
					float4 appendResult46 = (float4(( (( tex2DNode31 * tex2DNode35 * _Color3 * i.color )).rgb * _Emission3 ) , ( tex2DNode31.a * tex2DNode35.a * _Color3.a * i.color.a * _Opacity3 )));
					

					fixed4 col = appendResult46;
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
75;56;1440;826;2153.03;997.9271;2.069631;True;False
Node;AmplifyShaderEditor.CommentaryNode;8;-3613.381,185.3461;Inherit;False;1910.996;537.6462;Texture Mask;11;27;23;22;20;17;16;15;14;12;11;9;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector4Node;9;-3578.781,311.6853;Float;False;Property;_SpeedMask1Mask2;Speed Mask1/Mask2;4;0;Create;True;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-3850.669,-199.7266;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;13;-3393.43,-157.67;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;12;-3205.432,416.9444;Inherit;False;False;False;True;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;11;-3229.164,314.0785;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;15;-2821.584,291.3338;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;14;-2901.3,477.3543;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;16;-2635.325,262.7807;Inherit;True;Property;_Mask_1;Mask_1;2;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;18;-2883.658,-635.6392;Inherit;False;1037.896;533.6285;Textures movement;5;29;26;24;21;19;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;17;-2628.761,457.8105;Inherit;True;Property;_Mask_2;Mask_2;3;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-2244.6,244.8226;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;19;-2812.625,-531.5037;Inherit;False;Property;_SpeedMainNoise;Speed Main/Noise;8;0;Create;True;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;22;-2140.267,347.0824;Inherit;False;Property;_DisplayingMask;Displaying Mask;9;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;21;-2506.563,-548.0084;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;23;-2092.314,238.4411;Inherit;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;26;-2501.612,-457.6927;Inherit;False;False;False;True;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-1871.385,237.4498;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;24;-2052.762,-544.7054;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;30;-1493.297,-330.0653;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;29;-2059.625,-326.8332;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;31;-1302.041,-431.2116;Inherit;True;Property;_MainTex3;MainTex;0;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;34;-1272.982,-19.04937;Float;False;Property;_Color3;Color;6;1;[HDR];Create;True;0;0;False;0;False;0.5,0.5,0.5,1;0.5,0.5,0.5,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;35;-1288.278,-220.6533;Inherit;True;Property;_Noise3;Noise;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;33;-1234.022,153.9742;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-787.2051,-273.6779;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-1310.494,334.4826;Float;False;Property;_Opacity3;Opacity;7;0;Create;True;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;39;-512.9791,-198.2268;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;42;12.99543,-6.244282;Float;False;Property;_Emission3;Emission;5;0;Create;True;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-616.7142,-43.89297;Inherit;False;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;153.9962,-187.0974;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;46;404.0845,-130.5016;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;631,-141;Float;False;True;-1;2;ASEMaterialInspector;0;10;Simuranstudio/Particles/Black;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;True;2;False;-1;True;True;True;True;False;0;False;-1;False;False;False;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;13;0;10;1
WireConnection;13;1;10;2
WireConnection;12;0;9;0
WireConnection;11;0;9;0
WireConnection;15;0;13;0
WireConnection;15;2;11;0
WireConnection;14;0;13;0
WireConnection;14;2;12;0
WireConnection;16;1;15;0
WireConnection;17;1;14;0
WireConnection;20;0;16;0
WireConnection;20;1;17;0
WireConnection;21;0;19;0
WireConnection;23;0;20;0
WireConnection;26;0;19;0
WireConnection;27;0;23;0
WireConnection;27;1;22;0
WireConnection;24;0;13;0
WireConnection;24;2;21;0
WireConnection;30;0;24;0
WireConnection;30;1;27;0
WireConnection;29;0;13;0
WireConnection;29;2;26;0
WireConnection;31;1;30;0
WireConnection;35;1;29;0
WireConnection;37;0;31;0
WireConnection;37;1;35;0
WireConnection;37;2;34;0
WireConnection;37;3;33;0
WireConnection;39;0;37;0
WireConnection;44;0;31;4
WireConnection;44;1;35;4
WireConnection;44;2;34;4
WireConnection;44;3;33;4
WireConnection;44;4;41;0
WireConnection;45;0;39;0
WireConnection;45;1;42;0
WireConnection;46;0;45;0
WireConnection;46;3;44;0
WireConnection;0;0;46;0
ASEEND*/
//CHKSM=F01A0F250E860AF06FFF9B437FB1901E20621E29