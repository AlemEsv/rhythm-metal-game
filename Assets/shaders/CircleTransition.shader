Shader "Custom/CircleTransition"
{
Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (0,0,0,1)
        _Radius ("Circle Radius", Range(0, 1.5)) = 0
        _CenterX ("Center X", Range(0, 1)) = 0.5
        _CenterY ("Center Y", Range(0, 1)) = 0.5
        _AspectRatio ("Aspect Ratio", Float) = 1.77
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
            float _Radius;
            float _CenterX;
            float _CenterY;
            float _AspectRatio;

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
                float2 uv = IN.texcoord - float2(_CenterX, _CenterY);
                
                // Corregir aspecto para que el circulo no sea un ovalo
                uv.x *= _AspectRatio;

                // Calcular distancia al centro
                float dist = length(uv);

                // Si la distancia es menor al radio, es transparente (0 alpha)
                // Si es mayor, es negro (1 alpha)
                float alpha = smoothstep(_Radius, _Radius - 0.01, dist);
                
                // Invertimos
                float mask = smoothstep(_Radius - 0.01, _Radius + 0.01, dist);

                return fixed4(IN.color.rgb, mask * IN.color.a);
            }
            ENDCG
        }
    }
}
