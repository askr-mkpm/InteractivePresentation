Shader "test/geomSlide"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _emission ("emission", Range(1,2)) = 1
        _destRange ("destRange", Range(1,10)) = 1
        _alphaRange ("alphaRange", Range(1,2)) = 1.7
        _fadePos ("fadePos", Range(0,100)) = 25
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }
        
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom

            #include "UnityCG.cginc"
            #include "commonFunc.cginc"
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _emission;
            float _destRange;
            float _alphaRange;
            float _fadePos;

            struct appdata_t 
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            struct g2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float alpha : TEXCOORD1;
            };

            appdata_t vert(appdata_t v)
            {
                return v;
            }
            
            [maxvertexcount(3)]
            void geom(triangle appdata_t input[3], inout TriangleStream<g2f> stream)
            {
                float3 center = (input[0].vertex + input[1].vertex + input[2].vertex).xyz / 3;
            
                float3 vec1 = input[1].vertex - input[0].vertex;
                float3 vec2 = input[2].vertex - input[0].vertex;
                float3 normal = normalize(cross(vec1, vec2));
           
                float4 worldPos = mul(unity_ObjectToWorld, float4(center, 1.0));
                float3 dist = abs(length(_WorldSpaceCameraPos.z - worldPos.z) - _fadePos);
                
                float r = 2 * (rand(center.xy) - 0.5);
                float3 r3 = fixed3(r, r, r);

                [unroll]
                for (int i = 0; i < 3; ++i)
                {
                    appdata_t v = input[i];
            
                    g2f o;
                    
                    v.vertex.xyz += normalize(r3 * r3 * normal) * 0.01 * pow(dist, _destRange) * r ;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;
                    o.alpha = 1 - clamp(pow(dist * 0.07, _alphaRange), 0,1);
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
            
                    stream.Append(o);
                }
                stream.RestartStrip();
            }

            fixed4 frag (g2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                col *= _emission;
                col.a *= i.alpha;

                return col;
            }
            
            ENDCG
        }
    }
}
