Shader "Custom/TeamHoleRGB"
{
    Properties
    {
        // [HDR] etiketi sayesinde bu renge tıkladığında parlaklık ayarı (Intensity) açılır.
        [HDR] _TeamColor ("Takım Rengi (HDR)", Color) = (1, 0, 0, 1)
        
        // Halkanın genişliği ve kalınlık ayarları
        _MainRadius ("Halka Yarıçapı", Range(0.0, 0.5)) = 0.45
        _Thickness ("Kalınlık", Range(0.001, 0.1)) = 0.02
        _Softness ("Kenar Yumuşaklığı", Range(0.001, 0.05)) = 0.01
    }
    SubShader
    {
        // Bu etiketler objenin şeffaf olmasını ve diğer şeylerin üstüne çizilmesini sağlar.
        Tags { "RenderType"="Transparent" "Queue"="Transparent+10" }
        
        // "Additive Blending" - Işık gibi parlamasını sağlayan ayar.
        Blend SrcAlpha One 
        ZWrite Off // Derinlik yazma (Arkadakileri kapatmasın)
        Cull Off   // İki yüzü de görünsün (garanti olsun)

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

            float4 _TeamColor;
            float _MainRadius;
            float _Thickness;
            float _Softness;

            v2f vert (appdata v)
            {
                v2f o;
                // URP veya Built-in fark etmez çalışır
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // UV merkezini ortaya al (0.5, 0.5 -> 0,0)
                float2 center = i.uv - 0.5;
                // Merkeze olan uzaklığı ölç
                float dist = length(center);

                // Matematiksel olarak halka şeklini oluştur (Yumuşak kenarlı)
                float outerCircle = smoothstep(_MainRadius + _Softness, _MainRadius, dist);
                float innerCircle = smoothstep(_MainRadius - _Thickness, _MainRadius - _Thickness - _Softness, dist);
                
                // Dış daireden iç daireyi çıkarınca elimizde halka kalır (Alpha değeri)
                float ringAlpha = outerCircle - innerCircle;

                // Rengi ayarla
                float4 finalColor = _TeamColor;
                
                // Sadece halkanın olduğu yerleri boya, gerisini şeffaf yap
                finalColor.rgb *= ringAlpha;
                // Additive blend için alpha'yı da çarpıyoruz, parlaklığı artırmak için 2 ile çarptık.
                return finalColor * 2.0; 
            }
            ENDCG
        }
    }
}