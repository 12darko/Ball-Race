Shader "Custom/HoleRGB"
{
    Properties
    {
        _Color1 ("Renk 1 (Kırmızı)", Color) = (1, 0, 0, 1)
        _Color2 ("Renk 2 (Mavi)", Color) = (0, 0, 1, 1)
        _MainRadius ("Halka Genişliği", Range(0.0, 0.5)) = 0.45
        _Thickness ("Kalınlık", Range(0.0, 0.2)) = 0.05
        _Softness ("Yumuşaklık", Range(0.001, 0.1)) = 0.02
        _Speed ("Dönme Hızı", Range(-10, 10)) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha One // Additive Blend (Parlak Neon Efekti)
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

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

            float4 _Color1;
            float4 _Color2;
            float _MainRadius;
            float _Thickness;
            float _Softness;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // UV merkezini (0.5, 0.5) yap
                float2 center = i.uv - 0.5;
                float dist = length(center);
                
                // Açıyı hesapla (Renk geçişi için)
                float angle = atan2(center.y, center.x);
                
                // Halka şeklini oluştur (Matematiksel daire)
                // Dış sınır
                float outerCircle = smoothstep(_MainRadius + _Softness, _MainRadius, dist);
                // İç sınır
                float innerCircle = smoothstep(_MainRadius - _Thickness, _MainRadius - _Thickness - _Softness, dist);
                
                // İkisini çıkarınca elimizde sadece halka kalır
                float ring = outerCircle - innerCircle;

                // Renkleri karıştır (Dönen efekt)
                float timeShift = _Time.y * _Speed;
                float colorMix = sin(angle + timeShift); // -1 ile 1 arası
                colorMix = (colorMix + 1.0) * 0.5; // 0 ile 1 arasına çek
                
                float4 finalColor = lerp(_Color1, _Color2, colorMix);

                // Halkanın şeffaflığını uygula
                finalColor.a *= ring;
                
                // Siyah kısımları tamamen silmek için renkle çarp
                return finalColor * ring * 3.0; // *3.0 parlaklığı artırır
            }
            ENDCG
        }
    }
}