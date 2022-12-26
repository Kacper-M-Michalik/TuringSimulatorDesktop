#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix Projection;
float4 OverlayColor;

bool HasBaseTexture;
Texture2D<float4> BaseTexture : register(t0);
sampler BaseTextureSampler : register(s0);

//bool HasOverlayTexture;
//Texture2D<float4> OverlayTexture : register(t1);
//sampler OverlayTextureSampler : register(s1);

#define SAMPLE_TEXTURE(Name, texCoord)  Name.Sample(Name##Sampler, texCoord)

struct VertexShaderInput
{
	float4 Position : SV_Position;
	float2 TexCoord  : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoord  : TEXCOORD0;
};

VertexShaderOutput MainVS(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, Projection);
	output.TexCoord = input.TexCoord;

	return output;
}

float4 MainPS(VertexShaderOutput input) : SV_Target
{
	float4 Result = float4(OverlayColor.rgb * OverlayColor.a, OverlayColor.a);
	if (HasBaseTexture == true)
	{
		float4 Sample1 = SAMPLE_TEXTURE(BaseTexture, input.TexCoord);
		Result += float4(Sample1.rgb * Sample1.a, Sample1.a);
	}
	/*
	if (HasOverlayTexture == true)
	{
		float4 Sample2 = SAMPLE_TEXTURE(OverlayTexture, input.TexCoord);
		Result += float4(Sample2.rgb * Sample2.a, Sample2.a);
	}
	*/
	return Result;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};