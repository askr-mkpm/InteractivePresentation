Shader "test/geomSlide"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
                float3 dist = length(_WorldSpaceCameraPos - worldPos);

                [unroll]
                for (int i = 0; i < 3; ++i)
                {
                    appdata_t v = input[i];
            
                    g2f o;
            
                    v.vertex.xyz += normal * 0.0001 * pow(dist, 2);
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
            
                    stream.Append(o);
                }
                stream.RestartStrip();
            }

            fixed4 frag (g2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);

                return col;
            }
            
            ENDCG
        }
    }
}
