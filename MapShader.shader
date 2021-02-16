Shader "Custom/MapShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_MainColor("Main Color", Color) = (1, 1, 1, 1)
		_CircleColor("Circle Color", Color) = (1, 0, 0, 0)
		_CirclePosition("Circle Position", Vector) = (0, 0, 0, 0)
		_CircleMaxRadius("Circle MaxRadius", Range(0, 100)) = 0
		_CircleMinRadius("Circle MinRadius", Range(0, 100)) = 0
		_CircleThickness("Circle Thickness", Range(0, 100)) = 1
	}
		SubShader
		{
			CGPROGRAM
			#pragma surface SurfaceFunc Lambert

			sampler2D _MainTex;
			fixed4 _MainColor;
			fixed4 _CircleColor;
			float3 _CirclePosition;
			float _CircleThickness;
			float _CircleMaxRadius;
			float _CircleMinRadius;

			struct Input
			{
				float2 uv_MainTex;
				float3 worldPos;
			};

			void SurfaceFunc(Input IN, inout SurfaceOutput o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _MainColor;
				float dist = distance(_CirclePosition, IN.worldPos);

				if (dist < _CircleMaxRadius && dist > (_CircleMaxRadius - _CircleThickness))
				{
					float diff = (_CircleMaxRadius - dist) / _CircleThickness;

					fixed4 lerpColor = lerp(_CircleColor, c, diff);

					c.rgb = lerpColor.rgb;
					o.Albedo = lerpColor.rgb;
				}
				if (dist > _CircleMinRadius && dist <(_CircleMinRadius + _CircleThickness))
				{
					float diff = -(_CircleMinRadius - dist) / _CircleThickness;

					fixed4 lerpColor = lerp(_CircleColor, c, diff);

					o.Albedo = lerpColor.rgb;
				}
				else
				{
					o.Albedo = c.rgb;
				}

				o.Alpha = c.a;
			}
			ENDCG
		}
			Fallback "Diffuse"
}