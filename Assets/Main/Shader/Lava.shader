Shader "Custom/Lava"
{
    Properties
    {
        [Header(Texture Settings)]
        _MainTex ("Noise Texture", 2D) = "white" {}
        _Tiling ("Texture Tiling", Float) = 10.0
        
        [Header(Flow Settings)]
        _FlowSpeed ("Flow Speed (X, Y)", Vector) = (0.1, 0.05, 0, 0)

        [Header(Colors)]
        _MagmaColor ("Magma Color", Color) = (1, 0.2, 0, 1) // Koyu turuncu
        [HDR] _CrackColor ("Crack/Edge Color", Color) = (2, 1, 0.2, 1) // Parlak sarı (HDR)
        
        [Header(Toon Settings)]
        _Cutoff ("Crack Threshold", Range(0, 1)) = 0.5
        _EdgeWidth ("Edge Smoothness", Range(0, 0.2)) = 0.02
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Texture ve Sampler Tanımları (URP Standartı)
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float _Tiling;
                float2 _FlowSpeed;
                float4 _MagmaColor;
                float4 _CrackColor;
                float _Cutoff;
                float _EdgeWidth;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 1. Akış ve Tiling Ayarı
                // Zamanla kayan UV koordinatları
                float2 flowOffset = _FlowSpeed * _Time.y;
                float2 uv = (IN.uv * _Tiling) + flowOffset;

                // 2. Texture Okuma
                float noise = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).r;

                // 3. Toon/Keskin Geçiş Hesabı
                // smoothstep kullanarak noise değerini sert bir maskeye çeviriyoruz.
                // Bu işlem o görseldeki "hücreli" yapıyı oluşturur.
                float mask = smoothstep(_Cutoff, _Cutoff + _EdgeWidth, noise);

                // 4. Renk Karıştırma
                // Maske 0 ise Magma, 1 ise Çatlak rengi
                float4 finalColor = lerp(_MagmaColor, _CrackColor, mask);

                return finalColor;
            }
            ENDHLSL
        }
    }
}