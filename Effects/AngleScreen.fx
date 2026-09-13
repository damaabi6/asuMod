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
float2 uZoom;
float uRotation; // radians, +right / -left

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    // recenter to origin, correct for aspect ratio so rotation stays circular
    float aspect = uScreenResolution.x / uScreenResolution.y;
    float2 uv = coords - 0.5;
    uv.x *= aspect;

    float s = sin(uRotation);
    float c = cos(uRotation);
    float2x2 rot = float2x2(c, -s, s, c);
    uv = mul(rot, uv);

    uv.x /= aspect;
    uv += 0.5;

    // outside [0,1] after rotation, wrap
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