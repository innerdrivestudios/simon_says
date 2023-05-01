Shader "IDS/SpaceInvaderShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _EmissionColor ("Emmision Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        half4 _MainTex_TexelSize;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _EmissionColor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            //Get the base albedo and emissive color from the input texture and tinters
            fixed4 textureColor = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 albedoColor = textureColor * _Color;
            fixed4 emissiveColor = textureColor * _EmissionColor;

            //Use the UV (0..1) and _MainTex_TexelSize (a Vector4 with values (1 / width, 1 / height, width, height)) 
            //to get a pixel position
            fixed2 position = fixed2 (IN.uv_MainTex.x, 1- IN.uv_MainTex.y) * _MainTex_TexelSize.zw;
            //'snap' all positions to a 16 pixel grid, in other words within a grid cell all these values are the same
            position.x = int(position.x / 16) * 16;
            position.y = int(position.y / 16) * 16;
            //get 'distance' of this position along an angled vector (you could even rotate it)
            fixed2 direction = dot (position,fixed2(0.707, 0.707));
            //fixed2 direction = dot (position,fixed2(cos (_Time.x), sin(_Time.x)));
            
            //sin generates an intensity between -1 and 1, so we add 1 and * 0.5, this gives us a range of 0..1
            //but we don't want 0..1 we want at least a little bit of light, so we do 0.1 + 0.9 * the range of 1
            //which gives us an intensity of 0.1 to 1
            float intensity = 0.1f + 0.9 * 0.5 * (sin (direction + _Time.y*20) + 1);

            o.Albedo = albedoColor.rgb;
            o.Emission = emissiveColor.rgb * intensity;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = albedoColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
