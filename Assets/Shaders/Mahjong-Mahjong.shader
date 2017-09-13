Shader "Mahjong/Mahjong" {
Properties {
 _Color ("Main Color", Color) = (0,0,0,1)
 _SpecColor ("Specular Color", Color) = (0.5,0.5,0.5,1)
 _LightDir ("LightDir", Vector) = (0.5,0.5,0.5,1)
 _LightDir1 ("LightDir1", Vector) = (0.5,0.5,0.5,1)
 _Shininess ("Shininess", Range(0.01,50)) = 0.078125
 _Emission ("Emission", Range(0.01,1)) = 0.3
 _Intensity ("_Intensity", Float) = 1
 _MainTex ("Base (RGB) RefStrGloss (A)", 2D) = "white" {}
 _Mask ("MaskTexture", 2D) = "white" {}
}
SubShader { 
 Pass {
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 glstate_lightmodel_ambient;
uniform lowp vec4 _Color;
uniform lowp vec3 _LightDir;
uniform lowp vec3 _LightDir1;
varying highp vec2 xlv_TEXCOORD0;
varying mediump vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp float xlv_TEXCOORD4;
void main ()
{
  lowp vec3 ambientLighting_1;
  mediump vec3 lightDir1_2;
  mediump vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp float tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 0.0;
  tmpvar_6.xyz = normalize(_glesNormal);
  highp vec3 tmpvar_7;
  tmpvar_7 = normalize((tmpvar_6 * _World2Object).xyz);
  tmpvar_3 = tmpvar_7;
  highp vec3 tmpvar_8;
  tmpvar_8 = normalize((_LightDir - (_Object2World * _glesVertex).xyz));
  tmpvar_4 = tmpvar_8;
  lowp vec3 tmpvar_9;
  tmpvar_9 = normalize(_LightDir1);
  lightDir1_2 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = (glstate_lightmodel_ambient.xyz * _Color.xyz);
  ambientLighting_1 = tmpvar_10;
  mediump vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = (ambientLighting_1 + ((
    inversesqrt(dot (lightDir1_2, lightDir1_2))
   * _Color.xyz) * max (0.0, 
    dot (normalize(tmpvar_3), lightDir1_2)
  )));
  mediump float tmpvar_12;
  tmpvar_12 = inversesqrt(dot (tmpvar_4, tmpvar_4));
  tmpvar_5 = tmpvar_12;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = clamp (tmpvar_11, vec4(0.0, 0.0, 0.0, 0.0), vec4(1.0, 1.0, 1.0, 1.0));
  xlv_TEXCOORD4 = tmpvar_5;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform lowp vec4 _SpecColor;
uniform mediump float _Shininess;
uniform mediump float _Emission;
uniform mediump float _Intensity;
varying highp vec2 xlv_TEXCOORD0;
varying mediump vec3 xlv_TEXCOORD1;
varying mediump vec3 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp float xlv_TEXCOORD4;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp vec4 specularCol_2;
  mediump vec3 specularReflection_3;
  mediump vec3 tmpvar_4;
  tmpvar_4 = normalize(xlv_TEXCOORD1);
  mediump vec3 I_5;
  I_5 = -(xlv_TEXCOORD2);
  mediump float tmpvar_6;
  tmpvar_6 = pow (max (0.0, dot (
    (I_5 - (2.0 * (dot (tmpvar_4, I_5) * tmpvar_4)))
  , tmpvar_4)), _Shininess);
  highp vec3 tmpvar_7;
  tmpvar_7 = ((xlv_TEXCOORD4 * _SpecColor.xyz) * tmpvar_6);
  specularReflection_3 = tmpvar_7;
  mediump vec4 tmpvar_8;
  tmpvar_8.w = 1.0;
  tmpvar_8.xyz = specularReflection_3;
  specularCol_2 = tmpvar_8;
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_MainTex, xlv_TEXCOORD0);
  tmpvar_1 = (((
    (tmpvar_9 * xlv_TEXCOORD3)
   * _Intensity) + specularCol_2) + (tmpvar_9 * _Emission));
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp mat4 _World2Object;
uniform highp vec4 glstate_lightmodel_ambient;
uniform lowp vec4 _Color;
uniform lowp vec3 _LightDir;
uniform lowp vec3 _LightDir1;
out highp vec2 xlv_TEXCOORD0;
out mediump vec3 xlv_TEXCOORD1;
out mediump vec3 xlv_TEXCOORD2;
out mediump vec4 xlv_TEXCOORD3;
out highp float xlv_TEXCOORD4;
void main ()
{
  lowp vec3 ambientLighting_1;
  mediump vec3 lightDir1_2;
  mediump vec3 tmpvar_3;
  mediump vec3 tmpvar_4;
  highp float tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6.w = 0.0;
  tmpvar_6.xyz = normalize(_glesNormal);
  highp vec3 tmpvar_7;
  tmpvar_7 = normalize((tmpvar_6 * _World2Object).xyz);
  tmpvar_3 = tmpvar_7;
  highp vec3 tmpvar_8;
  tmpvar_8 = normalize((_LightDir - (_Object2World * _glesVertex).xyz));
  tmpvar_4 = tmpvar_8;
  lowp vec3 tmpvar_9;
  tmpvar_9 = normalize(_LightDir1);
  lightDir1_2 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = (glstate_lightmodel_ambient.xyz * _Color.xyz);
  ambientLighting_1 = tmpvar_10;
  mediump vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = (ambientLighting_1 + ((
    inversesqrt(dot (lightDir1_2, lightDir1_2))
   * _Color.xyz) * max (0.0, 
    dot (normalize(tmpvar_3), lightDir1_2)
  )));
  mediump float tmpvar_12;
  tmpvar_12 = inversesqrt(dot (tmpvar_4, tmpvar_4));
  tmpvar_5 = tmpvar_12;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = clamp (tmpvar_11, vec4(0.0, 0.0, 0.0, 0.0), vec4(1.0, 1.0, 1.0, 1.0));
  xlv_TEXCOORD4 = tmpvar_5;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform lowp vec4 _SpecColor;
uniform mediump float _Shininess;
uniform mediump float _Emission;
uniform mediump float _Intensity;
in highp vec2 xlv_TEXCOORD0;
in mediump vec3 xlv_TEXCOORD1;
in mediump vec3 xlv_TEXCOORD2;
in mediump vec4 xlv_TEXCOORD3;
in highp float xlv_TEXCOORD4;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp vec4 specularCol_2;
  mediump vec3 specularReflection_3;
  mediump vec3 tmpvar_4;
  tmpvar_4 = normalize(xlv_TEXCOORD1);
  mediump vec3 I_5;
  I_5 = -(xlv_TEXCOORD2);
  mediump float tmpvar_6;
  tmpvar_6 = pow (max (0.0, dot (
    (I_5 - (2.0 * (dot (tmpvar_4, I_5) * tmpvar_4)))
  , tmpvar_4)), _Shininess);
  highp vec3 tmpvar_7;
  tmpvar_7 = ((xlv_TEXCOORD4 * _SpecColor.xyz) * tmpvar_6);
  specularReflection_3 = tmpvar_7;
  mediump vec4 tmpvar_8;
  tmpvar_8.w = 1.0;
  tmpvar_8.xyz = specularReflection_3;
  specularCol_2 = tmpvar_8;
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture (_MainTex, xlv_TEXCOORD0);
  tmpvar_1 = (((
    (tmpvar_9 * xlv_TEXCOORD3)
   * _Intensity) + specularCol_2) + (tmpvar_9 * _Emission));
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
"!!GLES"
}
SubProgram "gles3 " {
"!!GLES3"
}
}
 }
}
Fallback "Unlit/Texture"
}