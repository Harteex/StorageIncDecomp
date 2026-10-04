// Deferred shading, pass 1: render the scene into the G-buffer (MRT).
//   COLOR0 = albedo (alpha 0), COLOR1 = normal * 0.5 + 0.5 (alpha 0), COLOR2 = depth z/w.
// Reconstructed from the Xbox 360 microcode (same structure as Riemer's XNA deferred
// shading tutorial). Sampler states are from the original effect: linear, wrap.

float4x4 xWorld;
float4x4 xView;
float4x4 xProjection;

texture xTexture;
sampler TextureSampler = sampler_state
{
	Texture = <xTexture>;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	MipFilter = LINEAR;
	AddressU = WRAP;
	AddressV = WRAP;
};

struct VertexToPixel
{
	float4 Position : POSITION;
	float3 Normal : TEXCOORD0;
	float4 ScreenPos : TEXCOORD1;
	float2 TexCoords : TEXCOORD2;
};

struct PixelToFrame
{
	float4 Color : COLOR0;
	float4 Normal : COLOR1;
	float4 Depth : COLOR2;
};

VertexToPixel MyVertexShader(float4 inPos : POSITION0, float3 inNormal : NORMAL0, float2 inTexCoords : TEXCOORD0)
{
	VertexToPixel output;
	float4x4 worldViewProjection = mul(mul(xWorld, xView), xProjection);
	output.Position = mul(inPos, worldViewProjection);
	output.Normal = mul(inNormal, (float3x3)xWorld); // not normalized, as in the original
	output.ScreenPos = output.Position;
	output.TexCoords = inTexCoords;
	return output;
}

PixelToFrame MyPixelShader(VertexToPixel input)
{
	PixelToFrame output;
	output.Color = float4(tex2D(TextureSampler, input.TexCoords).rgb, 0);
	output.Normal = float4(input.Normal * 0.5f + 0.5f, 0);
	output.Depth = input.ScreenPos.z / input.ScreenPos.w;
	return output;
}

technique MultipleTargets
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 MyVertexShader();
		PixelShader = compile ps_3_0 MyPixelShader();
	}
}
