Shader "Custom/Hole"
{
    Properties
    {
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "Queue" = "Geometry-1" // Burası kritik: Zeminden ÖNCE çizilmeli
            "RenderPipeline" = "UniversalPipeline" 
        }

        Pass
        {
            // Renk maskesini 0 yapıyoruz (Görünmezlik)
            ColorMask 0
            
            // Derinlik verisini yazıyoruz (Zemin burayı dolu sanacak)
            ZWrite On
            ZTest LEqual
        }
    }
}