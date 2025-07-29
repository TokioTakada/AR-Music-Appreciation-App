Shader "test/3DAudioSpectrum"
{
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Intensity("Emission Intensity", float) = 1
        _Width("width", float)=2
        _Amp("Amp", float) = 1
        _Param0("Param0", range(0,1)) = 0
        _Param1("Param1", range(0,1)) = 0
        _Param2("Param2", range(0,1)) = 0
        _Param3("Param3", range(0,1)) = 0
        _Param4("Param4", range(0,1)) = 0
        _Param5("Param5", range(0,1)) = 0
        _Param6("Param6", range(0,1)) = 0
        _Param7("Param7", range(0,1)) = 0
        _Param8("Param8", range(0,1)) = 0
        _Param9("Param9", range(0,1)) = 0
        _Param10("Param10", range(0,1)) = 0
        _Param11("Param11", range(0,1)) = 0
        _Param12("Param12", range(0,1)) = 0
        _Param13("Param13", range(0,1)) = 0
        _Param14("Param14", range(0,1)) = 0
        _Param15("Param15", range(0,1)) = 0
        }

        SubShader{

            Tags { "RenderType" = "Opaque" }

            CGPROGRAM
            #pragma surface surf Lambert vertex:vert

            struct Input {
                float2 uv_MainTex;
                float4 color;
            };

            sampler2D _MainTex;
            float _Param0, _Param1, _Param2, _Param3, _Param4, _Param5, _Param6, _Param7, _Param8, _Param9, _Param10, _Param11, _Param12, _Param13, _Param14, _Param15;
            float _Amp, _Intensity, _Width;

            void vert(inout appdata_full v) {
                float pos = v.vertex.x / _Width + 0.5;
                float cond01 = step(pos , 1. / 16);
                float cond02 = step(pos , 2. / 16);
                float cond03 = step(pos , 3. / 16);
                float cond04 = step(pos , 4. / 16);
                float cond05 = step(pos , 5. / 16);
                float cond06 = step(pos , 6. / 16);
                float cond07 = step(pos , 7. / 16);
                float cond08 = step(pos , 8. / 16);
                float cond09 = step(pos , 9. / 16);
                float cond10 = step(pos , 10. / 16);
                float cond11 = step(pos , 11. / 16);
                float cond12 = step(pos , 12. / 16);
                float cond13 = step(pos , 13. / 16);
                float cond14 = step(pos , 14. / 16);
                float cond15 = step(pos , 15. / 16);
                v.vertex.y =cond01 * v.vertex.y * _Param15 * _Amp + (1 - cond01) * v.vertex.y;
                v.vertex.y =(1 -cond01) * cond02 * v.vertex.y * _Param14 * _Amp + (1 - (1 - cond01) * cond02) * v.vertex.y;
                v.vertex.y =(1 -cond02) * cond03 * v.vertex.y * _Param13 * _Amp + (1 - (1 - cond02) * cond03) * v.vertex.y;
                v.vertex.y =(1 -cond03) * cond04 * v.vertex.y * _Param12 * _Amp + (1 - (1 - cond03) * cond04) * v.vertex.y;
                v.vertex.y =(1 -cond04) * cond05 * v.vertex.y * _Param11 * _Amp + (1 - (1 - cond04) * cond05) * v.vertex.y;
                v.vertex.y =(1 -cond05) * cond06 * v.vertex.y * _Param10 * _Amp + (1 - (1 - cond05) * cond06) * v.vertex.y;
                v.vertex.y =(1 -cond06) * cond07 * v.vertex.y * _Param9 * _Amp + (1 - (1 - cond06) * cond07) * v.vertex.y;
                v.vertex.y =(1 -cond07) * cond08 * v.vertex.y * _Param8 * _Amp + (1 - (1 - cond07) * cond08) * v.vertex.y;
                v.vertex.y =(1 -cond08) * cond09 * v.vertex.y * _Param7 * _Amp + (1 - (1 - cond08) * cond09) * v.vertex.y;
                v.vertex.y =(1 -cond09) * cond10 * v.vertex.y * _Param6 * _Amp + (1 - (1 - cond09) * cond10) * v.vertex.y;
                v.vertex.y =(1 -cond10) * cond11 * v.vertex.y * _Param5 * _Amp + (1 - (1 - cond10) * cond11) * v.vertex.y;
                v.vertex.y =(1 -cond11) * cond12 * v.vertex.y * _Param4 * _Amp + (1 - (1 - cond11) * cond12) * v.vertex.y;
                v.vertex.y =(1 -cond12) * cond13 * v.vertex.y * _Param3 * _Amp + (1 - (1 - cond12) * cond13) * v.vertex.y;
                v.vertex.y =(1 -cond13) * cond14 * v.vertex.y * _Param2 * _Amp + (1 - (1 - cond13) * cond14) * v.vertex.y;
                v.vertex.y =(1 -cond14) * cond15 * v.vertex.y * _Param1 * _Amp + (1 - (1 - cond14) * cond15) * v.vertex.y;
                v.vertex.y =(1 -cond15) * v.vertex.y * _Param0 * _Amp + cond15 * v.vertex.y;
            }

            void surf(Input IN, inout SurfaceOutput o) {
                o.Albedo = tex2D(_MainTex, IN.uv_MainTex.yx).rgb * _Intensity;
            }
            ENDCG
        }
            Fallback "Diffuse"
    }
