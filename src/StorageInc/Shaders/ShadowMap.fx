// Shadow map: depth (z/w) of the scene as seen from the spot light.
// xView/xProjection are the light's matrices. Reconstructed from the Xbox 360 microcode.
// xTexture is declared (the game sets it) but unused, as in the original.

float4x4 xWorld;
float4x4 xView;
float4x4 xProjection;
texture xTexture;

struct VertexToPixel
{
	float4 Position : POSITION;
	float4 ScreenPos : TEXCOORD1;
};

VertexToPixel MyVertexShader(float4 inPos : POSITION0)
{
	VertexToPixel output;
	float4x4 worldViewProjection = mul(mul(xWorld, xView), xProjection);
	output.Position = mul(inPos, worldViewProjection);
	output.ScreenPos = output.Position;
	return output;
}

float4 MyPixelShader(VertexToPixel input) : COLOR0
{
	return input.ScreenPos.z / input.ScreenPos.w;
}

technique ShadowMap
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 MyVertexShader();
		PixelShader = compile ps_3_0 MyPixelShader();
	}
}
