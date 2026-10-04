// Deferred shading, pass 2: add one shadowed spot light to the shading map.
// Drawn as a full-screen quad (positions already in clip space).
// Reconstructed from the Xbox 360 microcode, including its literal constants
// (2, -1, 0.5 and the 0.01 shadow bias). Note the result is not saturated: surfaces
// facing away from the light subtract from the shading, exactly like the original.
// Sampler states are from the original effect: G-buffer point/mirror, shadow and
// previous-shading linear/mirror.

float4x4 xViewProjectionInv;
float4x4 xLightViewProjection;
float xLightStrength;
float3 xLightPosition;
float3 xConeDirection;
float xConeAngle;
float xConeDecay;

texture xNormalMap;
sampler NormalMapSampler = sampler_state
{
	Texture = <xNormalMap>;
	MagFilter = POINT;
	MinFilter = POINT;
	MipFilter = LINEAR;
	AddressU = MIRROR;
	AddressV = MIRROR;
};

texture xDepthMap;
sampler DepthMapSampler = sampler_state
{
	Texture = <xDepthMap>;
	MagFilter = POINT;
	MinFilter = POINT;
	MipFilter = LINEAR;
	AddressU = MIRROR;
	AddressV = MIRROR;
};

texture xShadowMap;
sampler ShadowMapSampler = sampler_state
{
	Texture = <xShadowMap>;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	MipFilter = LINEAR;
	AddressU = MIRROR;
	AddressV = MIRROR;
};

texture xPreviousShadingContents;
sampler PreviousSampler = sampler_state
{
	Texture = <xPreviousShadingContents>;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
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
	// Reconstruct the world position from the depth map.
	float depth = tex2D(DepthMapSampler, input.TexCoord).r;
	float4 screenPos;
	screenPos.x = input.TexCoord.x * 2.0f - 1.0f;
	screenPos.y = -(input.TexCoord.y * 2.0f - 1.0f);
	screenPos.z = depth;
	screenPos.w = 1.0f;
	float4 worldPos = mul(screenPos, xViewProjectionInv);
	worldPos /= worldPos.w;

	// Look the position up in the light's shadow map.
	float4 lightScreenPos = mul(worldPos, xLightViewProjection);
	lightScreenPos /= lightScreenPos.w;
	float2 lightSamplePos;
	lightSamplePos.x = lightScreenPos.x * 0.5f + 0.5f;
	lightSamplePos.y = -lightScreenPos.y * 0.5f + 0.5f;
	float distanceStoredInDepthMap = tex2D(ShadowMapSampler, lightSamplePos).r;
	bool lit = distanceStoredInDepthMap > lightScreenPos.z - 0.01f;

	float3 normal = normalize(tex2D(NormalMapSampler, input.TexCoord).rgb * 2.0f - 1.0f);
	float3 lightDirection = normalize(worldPos.xyz - xLightPosition);
	float coneDot = dot(lightDirection, normalize(xConeDirection));
	bool inCone = coneDot >= xConeAngle;

	float shading = 0.0f;
	if (lit && inCone)
	{
		float coneAttenuation = pow(abs(coneDot), xConeDecay);
		shading = dot(normal, -lightDirection) * (xLightStrength * coneAttenuation);
	}

	return tex2D(PreviousSampler, input.TexCoord) + shading;
}

technique DeferredSpotLight
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 MyVertexShader();
		PixelShader = compile ps_3_0 MyPixelShader();
	}
}
