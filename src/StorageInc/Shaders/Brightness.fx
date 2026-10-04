// Brightness post-process, applied by ScreenManager.PostProcessImage via SpriteBatch.
// Reconstructed from the Xbox 360 microcode: one texture fetch, then
//   oC0.rgb = tex.rgb * Intensity; oC0.a = tex.a
// Intensity = GlobalSettings.Brightness / 5 (set by the game).

float Intensity;

sampler2D clrSampler : register(s0);

float4 BrightnessPS(float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(clrSampler, texCoord);
	return float4(color.rgb * Intensity, color.a);
}

technique Technique1
{
	pass Pass1
	{
		// Pixel shader only: SpriteBatch supplies the vertex shader (hence ps_2_0).
		PixelShader = compile ps_2_0 BrightnessPS();
	}
}
