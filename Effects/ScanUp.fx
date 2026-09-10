sampler uImage0 : register(s0);
float uProgress; //0-1
float uScanHeight;
float4 color1;
float4 color2;
float alpha;
bool bloom;
float uTime;

float4 EffectFunction(float2 coords : TEXCOORD0, float4 color : COLOR0) : COLOR0
{
    float4 tex = tex2D(uImage0, coords + float2(0, uTime * 0.15));

    float scanY = 1.0 - uProgress; // bottom-to-top

    float dist = abs(coords.y - scanY);
    float mask = 1.0 - smoothstep(0, uScanHeight * 0.5, dist);
    
    float4 colorB = float4(0, 0, 0, 0);
    if (bloom && tex.r > 0.8)
    {
        float z = (tex.r - 0.8) * 3.5;
        colorB = float4(z, z, z, 1);
    }
    float edgeFade = sin(coords.x * 3.141);
    float edgeFade2 = sin(coords.y * 3.141);
    tex.a *= mask;
    
    return (lerp(color1, color2, tex.r) + colorB) * float4(1, 1, 1, tex.r * alpha * tex.a * edgeFade * edgeFade2);
}

technique Technique1
{
    pass EffectPass
    {
        PixelShader = compile ps_2_0 EffectFunction();
    }
}