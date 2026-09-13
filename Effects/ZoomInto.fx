sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float uZoom; 
float uZoomAmnt; // 1.0 = no zoom, >1.0 = zoomed in
float2 uZoomPoint; // normalized [0,1] screen-space target, e.g. (0.5, 0.5) = center

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    // pull sample coords toward the zoom point instead of screen center
    float2 uv = uZoomPoint + (coords - uZoomPoint) / uZoomAmnt;

    if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
        return float4(0, 0, 0, 1);

    return tex2D(uImage0, uv);
}

technique Technique1
{
    pass EffectPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}