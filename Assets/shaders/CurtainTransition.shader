Shader "Custom/CurtainTransition"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,1)
        _OpenAmount ("Apertura", Range(0, 1)) = 0 // 0 = Cerrado, 1 = Abierto
        _Smoothness ("Suavidad en bordes", Range(0, 0.5)) = 0.1
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
            #include "UnityUI.cginc"

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
                float4 worldPosition : TEXCOORD1;
            };

            fixed4 _Color;
            float _OpenAmount;
            float _Smoothness;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // Coordenadas UV (0 a 1)
                float2 uv = IN.texcoord;

                // Calculamos la distancia desde el centro horizontal (0.5)
                float distFromCenter = abs(uv.x - 0.5);

                // El hueco se abre segun _OpenAmount. 
                // Dividimos por 2 porque abrimos hacia ambos lados.
                float currentGap = _OpenAmount * 0.5;

                // Creamos la máscara. 
                // Si la distancia al centro es MENOR que el hueco actual -> Transparente (0)
                // Si es MAYOR -> Opaco (1)
                float mask = smoothstep(currentGap, currentGap + _Smoothness, distFromCenter);

                return fixed4(IN.color.rgb, mask * IN.color.a);
            }
            ENDCG
        }
    }
}