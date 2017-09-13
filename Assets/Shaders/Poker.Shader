Shader "Self/Poker" {
	Properties
	{
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)				
		_MainTex("Base (RGB)", 2D) = "white" {}            //贴图 选项："white","black","gray","bump"中的一个
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }                    //声明在渲染非透明物体时调用
		LOD 200                                            //允许100-600之间  这个数值决定了我们能用什么样的Shader
		CGPROGRAM                                          //“Cg/HLSL”语言
														   //#pragma surface surf Lambert 声明表面着色，
														   //且函数名称为“surf”,
														   //使用的光照模型是：“Lambert”（也就是普通的diffuse）
		#pragma surface surf Lambert					   //Lambert 表示漫反射材质

		sampler2D _MainTex;                                //再次声明变量
		float4 _Color;

		struct Input                                       //输入的结构体
		{
			float2 uv_MainTex;
		};

		//Shader 主程序函数
		void surf(Input IN, inout SurfaceOutput o)		   //CG规定固定了方法的“签名”
		{
			half4 c		= tex2D(_MainTex, IN.uv_MainTex);
			o.Emission	= c.rgba * _Color;				   //表面自发光,不接受反射光照
			//o.Albedo	= c.rgb
			//o.Alpha	= c.a;	
		}
		ENDCG
	}

	FallBack "Diffuse"
}
