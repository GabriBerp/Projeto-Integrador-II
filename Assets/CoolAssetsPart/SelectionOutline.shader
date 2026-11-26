Shader "Custom/SelectionOutline"
{
    Properties
    {
        _OutlineColor ("Color", Color) = (1, 0.85, 0.2, 1)
        _OutlineWidth ("Width (world units)", Float) = 0.03
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1" }

        Pass
        {
            Cull Front
            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _OutlineColor;
            float _OutlineWidth;

            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; };

            float4 vert(appdata v) : SV_POSITION
            {
                // Extrusão em espaço de mundo: a largura não depende da escala do objeto
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                worldPos += worldNormal * _OutlineWidth;
                return mul(UNITY_MATRIX_VP, float4(worldPos, 1));
            }

            fixed4 frag() : SV_Target { return _OutlineColor; }
            ENDCG
        }
    }
}