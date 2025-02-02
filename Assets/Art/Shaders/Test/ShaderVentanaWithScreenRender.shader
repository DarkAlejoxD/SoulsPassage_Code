Shader "Unlit/LluviaVentana"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Size("Size", float) = 1
        _T("Time", float) = 1
        _Distortion("Distortion", range(-5, 5)) = 1
        _Blur("Blur", range(0, 1)) = 1
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" }
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                // make fog work
                #pragma multi_compile_fog
                #define S(a, b, t) smoothstep(a, b, t)
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    //float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float4 posPantalla : TEXCOORD0;
                    float2 uv : TEXCOORD1; //Cambia el registro para que sean independientes?
                    UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _Size, _T, _Distortion, _Blur;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.posPantalla = ComputeScreenPos(o.vertex); //Muestra la textura seg�n el espacio de la pantalla
                    o.uv = TRANSFORM_TEX(v.vertex.xy, _MainTex); //Usa coordenadas UV b�sicas 
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                float N21(float2 p)
                {
                    p = frac(p * float2(123.34, 345.45));
                    p += dot(p, p + 34.345);
                    return frac(p.x * p.y);
                }
                float3 Layer(float2 UV, float t)
                {
                    float2 aspect = float2(2, 1);
                    float2 uv = UV * _Size * aspect;
                    uv.y += t * .25;
                    float2 gv = frac(uv) - .5;
                    float2 id = floor(uv);

                    float n = N21(id); //da un n�mero random entre 0 y 1
                    t += n * 6.2831;

                    float w = UV.y * 10;
                    float x = (n - .5) * .8; // -4 .4
                    x += (.4 - abs(x)) * sin(3 * w) * pow(sin(w), 6) * .45;

                    float y = -sin(t + sin(t + sin(t) * .5)) * .45;
                    y -= (gv.x - x) * (gv.x - x); //para cambiar la forma del c�rculo

                    float2 dropPos = (gv - float2(x,y)) / aspect;
                    float drop = S(.05, .03, length(dropPos));

                    float2 trailPos = (gv - float2(x, t * .25)) / aspect;
                    trailPos.y = (frac(trailPos.y * 8) - .5) / 8;
                    float trail = S(.03, .01, length(trailPos));
                    float fogTrail = S(-.05, .05, dropPos.y);
                    fogTrail *= S(.5, y, gv.y);
                    trail *= fogTrail;
                    fogTrail *= S(.05, .04, abs(dropPos.x));

                    //col += fogTrail * .5;
                    //col += trail;
                    //col += drop;

                    //col *= 0; col.rg += dropPos;
                    float2 offs = drop * dropPos + trail * trailPos;
                    //if (gv.x > .48 || gv.y > .49) col = float4(1, 0, 0, 1);
                    //col *= 0; col += N21(id); //col.rg = id*.1;

                    return float3(offs, fogTrail);
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float t = fmod(_Time.y + _T, 7200);

                    float4 col = 0;

                    float3 drops = Layer(i.uv, t);
                    drops += Layer(i.uv * 1.23 + 7.54, t);
                    drops += Layer(i.uv * 1.35 + 1.54, t);
                    drops += Layer(i.uv * 1.57 - 7.54, t);

                    float blur = _Blur * 7 * (1 - drops.z);

                    float2 uv = i.posPantalla.xy / i.posPantalla.w;
                    col = tex2Dlod(_MainTex, float4(uv + drops.xy * _Distortion, 0, blur));
                    return col;
                }
                ENDCG
            }
        }
}
