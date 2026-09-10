sampler uImage : register(s0);
float4 color1;
float4 color2;
float alpha;
float time;
float trailFade;

float4 EffectFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 scrolledCoords = coords + float2(time * 0.75, 0);
    float4 colory = tex2D(uImage, scrolledCoords);
    float fade = coords.x * trailFade;
    float edgeFade = sin(coords.y * 3.14159);
    return lerp(color1, color2, colory.r) * float4(1, 1, 1, colory.r * alpha * fade * edgeFade);
}
technique Technique1
{
    pass EffectPass
    {
        PixelShader = compile ps_2_0 EffectFunction();
    }
}