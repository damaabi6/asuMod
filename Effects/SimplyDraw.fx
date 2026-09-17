sampler uImage : register(s0);
float alpha;

float4 EffectFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 colory = tex2D(uImage, coords);
    return colory * float4(1, 1, 1, alpha);
}

technique Technique1
{
    pass EffectPass
    {
        PixelShader = compile ps_2_0 EffectFunction();
    }
}