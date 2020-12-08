Shader "Unlit/CloudShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha ("Alpha",float) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"= "Transparent" }

        Pass
        {
            ZTest Off
            Blend SrcAlpha OneMinusSrcAlpha
    

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            uniform float time;
            uniform float speed;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float, _Alpha)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                if (speed == 0)
                    speed = 0.1;

                o.uv.x += time * speed;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float alpha = UNITY_ACCESS_INSTANCED_PROP(Props, _Alpha);
                if(col.w != 0)
                    col.w = alpha;
                return col;
            }
            ENDCG
        }
    }
}
