Shader "IDS/EdgeShader"
{
    Properties
    {
        _Range("Range", Range(0,2)) = 1.0
        [HDR] _Color("Color", Color) = (1,1,1,1)
    }
        SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 100
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _Range;
            fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float sineBasedAlphaFadeOut(float x, float y, float baseLine, float phaseMultiplier, float timeOffset, float max) {
                //first we generate a base sine wave (with values from -1 to 1) but remapped to 0..1
                float baseSine = (sin((x + timeOffset) * 2 * 3.14159265 * phaseMultiplier) + 1) * 0.5;
                //now shift the baseSine up by the baseLine and make sure the total is still 0..1 muliplied by max
                float shiftAndCapped = (baseLine + baseSine * (1-baseLine)) * max;

                //Now we know the value of the sinewave, so we could for example say:
                //if the actual input y is smaller return an intensity of 1, otherwise return an intensity of 0
                //However that is not what we want, we want to map the input y to an alpha, which should be
                //1 when y == 0 and 0 when y == the calculated shiftAndCapped (ie sine) value.
                //How do we d that? Like this (just fill in a y from 0 to shiftAndCapped and note the result):
                return clamp(-1 / shiftAndCapped * y + 1, 0, 1);
            }


            fixed4 frag(v2f i) : SV_Target
            {
                //get two different waves with different max ranges and phases and mix'em
                float alpha1 = sineBasedAlphaFadeOut(i.uv.x, i.uv.y, 0.2, 5, _Time.x * 0.1, 0.8 * _Range);
                float alpha2 = sineBasedAlphaFadeOut(i.uv.x, i.uv.y, 0.2, 10, 1+_Time.x * -0.3, 0.5 * _Range);
                fixed4 color = _Color;
                color.a = pow(alpha1, 2) * 0.5 + pow(alpha2, 2) * 0.5;
                
                return color;
            }

            ENDCG
        }
    }
}
