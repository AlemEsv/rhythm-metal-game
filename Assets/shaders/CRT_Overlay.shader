Shader "Custom/CRT_Overlay"
{
    Properties
    {
        _Color ("Tint Color", Color) = (1,1,1,1)
        
        [Header(Scanlines)]
        _ScanlineColor ("Color Lineas", Color) = (0,0,0,1)
        _ScanlineCount ("Cantidad de Lineas", Float) = 800
        _ScanlineIntensity ("Intensidad", Range(0, 1)) = 0.3
        _ScanlineSpeed ("Velocidad Movimiento", Float) = 1.0

        [Header(Vignette)]
        _VignetteIntensity ("Intensidad vignette", Range(0, 3)) = 1.2
        _VignetteSmoothness ("Suavidad vignette", Range(0.1, 5)) = 2.5
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
            };

            fixed4 _Color;
            
            // Scanlines
            fixed4 _ScanlineColor;
            float _ScanlineCount;
            float _ScanlineIntensity;
            float _ScanlineSpeed;

            // Vignette
            float _VignetteIntensity;
            float _VignetteSmoothness;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // VIÑETA
                float2 uv = IN.texcoord;
                float2 coord = (uv - 0.5) * 2.0;
                float dist = length(coord);

                float vignette = pow(dist, _VignetteSmoothness) * _VignetteIntensity;
                vignette = saturate(vignette); // Clampear entre 0 y 1

                // 2SCANLINES
                // efecto de refresco de pantalla
                float scanline = sin(uv.y * _ScanlineCount + _Time.y * _ScanlineSpeed);
                scanline = (scanline + 1.0) * 0.5; 
                
                // intensidad
                float scanlineEffect = (1.0 - scanline) * _ScanlineIntensity;

                // COMBINAR
                fixed4 finalColor = fixed4(0,0,0,0);
                
                // Agregamos el color de la viñeta (bordes oscuros)
                // Usamos el alpha para oscurecer la pantalla
                float finalAlpha = max(vignette, scanlineEffect);
                
                finalColor.rgb = _ScanlineColor.rgb;
                finalColor.a = finalAlpha * IN.color.a;

                return finalColor;
            }
            ENDCG
        }
    }
}