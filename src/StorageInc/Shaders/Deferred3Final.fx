// Deferred shading, pass 3: combine albedo with the accumulated shading.
//   color = albedo * (xAmbient + shading.r)   (all four channels, as in the original)
// Reconstructed from the Xbox 360 microcode. Samplers: point/mirror (original states).

float xAmbient;

texture xColorMap;
sampler ColorMapSampler = sampler_state
{
	Texture = <xColorMap>;
	MagFilter = POINT;
	MinFilter = POINT;
	MipFilter = LINEAR;
	AddressU = MIRROR;
	AddressV = MIRROR;
};

texture xShadingMap;
sampler ShadingMapSampler = sampler_state
{
	Texture = <xShadingMap>;
	MagFilter = POINT;
	MinFilter = POINT;
	MipFilter = LINEAR;
	AddressU = MIRROR;
	AddressV = MIRROR;
};

struct VertexToPixel
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
};

VertexToPixel MyVertexShader(float4 inPos : POSITION0, float2 texCoord : TEXCOORD0)
{
	VertexToPixel output;
	output.Position = inPos;
	output.TexCoord = texCoord;
	return output;
}

float4 MyPixelShader(VertexToPixel input) : COLOR0
{
	float4 color = tex2D(ColorMapSampler, input.TexCoord);
	float shading = tex2D(ShadingMapSampler, input.TexCoord).r;
	return color * (xAmbient + shading);
}

technique CombineColorAndShading
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 MyVertexShader();
		PixelShader = compile ps_3_0 MyPixelShader();
	}
}
