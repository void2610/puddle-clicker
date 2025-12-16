Shader "PuddleClicker/Ripple"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (0.3, 0.5, 0.7, 0.8)
        _RippleColor ("Ripple Color", Color) = (1, 1, 1, 0.5)
        _RippleWidth ("Ripple Width", Range(0.01, 0.2)) = 0.05

        // 波紋1
        _Ripple1Origin ("Ripple 1 Origin", Vector) = (0.5, 0.5, 0, 0)
        _Ripple1Params ("Ripple 1 Params (Progress, Scale, _, _)", Vector) = (0, 1, 0, 0)

        // 波紋2
        _Ripple2Origin ("Ripple 2 Origin", Vector) = (0.5, 0.5, 0, 0)
        _Ripple2Params ("Ripple 2 Params (Progress, Scale, _, _)", Vector) = (0, 1, 0, 0)

        // 波紋3
        _Ripple3Origin ("Ripple 3 Origin", Vector) = (0.5, 0.5, 0, 0)
        _Ripple3Params ("Ripple 3 Params (Progress, Scale, _, _)", Vector) = (0, 1, 0, 0)

        // 波紋4
        _Ripple4Origin ("Ripple 4 Origin", Vector) = (0.5, 0.5, 0, 0)
        _Ripple4Params ("Ripple 4 Params (Progress, Scale, _, _)", Vector) = (0, 1, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BaseColor;
            float4 _RippleColor;
            float _RippleWidth;

            float4 _Ripple1Origin;
            float4 _Ripple1Params;
            float4 _Ripple2Origin;
            float4 _Ripple2Params;
            float4 _Ripple3Origin;
            float4 _Ripple3Params;
            float4 _Ripple4Origin;
            float4 _Ripple4Params;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // 単一の波紋を計算
            float calcRipple(float2 uv, float2 origin, float progress, float scale)
            {
                if (progress <= 0 || progress >= 1) return 0;

                float dist = distance(uv, origin);
                float radius = progress * scale;
                float ripple = 1 - abs(dist - radius) / _RippleWidth;
                ripple = saturate(ripple);

                // フェードアウト
                float fade = 1 - progress;
                return ripple * fade;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = _BaseColor;

                // 各波紋を計算
                float ripple = 0;
                ripple += calcRipple(i.uv, _Ripple1Origin.xy, _Ripple1Params.x, _Ripple1Params.y);
                ripple += calcRipple(i.uv, _Ripple2Origin.xy, _Ripple2Params.x, _Ripple2Params.y);
                ripple += calcRipple(i.uv, _Ripple3Origin.xy, _Ripple3Params.x, _Ripple3Params.y);
                ripple += calcRipple(i.uv, _Ripple4Origin.xy, _Ripple4Params.x, _Ripple4Params.y);

                ripple = saturate(ripple);

                // 波紋カラーをブレンド
                col = lerp(col, _RippleColor, ripple * _RippleColor.a);

                return col;
            }
            ENDCG
        }
    }
}
