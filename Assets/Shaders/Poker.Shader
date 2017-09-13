Shader "Self/Poker" {
	Properties
	{
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)				
		_MainTex("Base (RGB)", 2D) = "white" {}            //��ͼ ѡ�"white","black","gray","bump"�е�һ��
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }                    //��������Ⱦ��͸������ʱ����
		LOD 200                                            //����100-600֮��  �����ֵ��������������ʲô����Shader
		CGPROGRAM                                          //��Cg/HLSL������
														   //#pragma surface surf Lambert ����������ɫ��
														   //�Һ�������Ϊ��surf��,
														   //ʹ�õĹ���ģ���ǣ���Lambert����Ҳ������ͨ��diffuse��
		#pragma surface surf Lambert					   //Lambert ��ʾ���������

		sampler2D _MainTex;                                //�ٴ���������
		float4 _Color;

		struct Input                                       //����Ľṹ��
		{
			float2 uv_MainTex;
		};

		//Shader ��������
		void surf(Input IN, inout SurfaceOutput o)		   //CG�涨�̶��˷����ġ�ǩ����
		{
			half4 c		= tex2D(_MainTex, IN.uv_MainTex);
			o.Emission	= c.rgba * _Color;				   //�����Է���,�����ܷ������
			//o.Albedo	= c.rgb
			//o.Alpha	= c.a;	
		}
		ENDCG
	}

	FallBack "Diffuse"
}