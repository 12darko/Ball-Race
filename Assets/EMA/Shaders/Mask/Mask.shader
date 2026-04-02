Shader "Custom/Mask"
{
   Subshader {
      Tags { "Queue" = "Transparent+1"}
       
       Pass{
           Blend Zero one
           } 
       }
}
