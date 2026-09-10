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
float uOpacity;// intensity
float uTime;
float uIntensity;// falloff
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;
float uMaxDist;

float4 ZoomBlur(float2 coords : TEXCOORD0) : COLOR0
{
    float2 targetCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 centreCoords = (coords - targetCoords) * (uScreenResolution / uScreenResolution.y);
    float dist = dot(centreCoords, centreCoords);
    float clampedDist = min(dist, uMaxDist);
    float strength = uOpacity * clampedDist * uIntensity;

    float4 result = float4(0, 0, 0, 0);
    int samples = 12;
    for (int i = 0; i < samples; i++)
    {
        float t = (float) i / (samples - 1) - 0.5;
        float2 sampleCoords = coords + (centreCoords * t * strength);
        result += tex2D(uImage0, sampleCoords);
    }
    return result / samples;
}

technique Technique1
{
    pass EffectPass
    {
        PixelShader = compile ps_3_0 ZoomBlur();
    }
}