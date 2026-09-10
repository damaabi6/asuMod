//art Attack shader
sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
matrix uWorldViewProjection;
float shouldFlip;
float4 uShaderSpecificData;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    float4 pos = mul(input.Position, uWorldViewProjection);
    output.Position = pos;
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = input.Color;
    float2 coords = input.TextureCoordinates;

     // Account for texture distortion artifacts.
    coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;
    
    float4 fadeMapColor = tex2D(uImage1, coords - float2(uTime * 1.5, 0));
    float opacity = fadeMapColor.r;
    
    float4 colorB = float4(0, 0, 0, 0);
    if (fadeMapColor.r > 0.8)
    {
        float z = (fadeMapColor.r - 0.8) * 3.5;
        colorB = float4(z, z, z, 1);
    }
    
    float crossFade = sin(coords.y * 3.141) * 1.4;

    return (fadeMapColor * color) * float4(1, 1, 1, opacity) * crossFade;
}

technique Technique1
{
    pass TrailPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
