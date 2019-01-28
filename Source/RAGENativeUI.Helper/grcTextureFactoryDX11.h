#pragma once

#include "grcTextureDX11.h"

namespace rage
{
	struct grcImage
	{
		enum class Format : uint32
		{
			B8G8R8A8_UNORM = 11,
		};

		uint16 m_nWidth;
		uint16 m_nHeight;
		Format m_nFormat;
		uint32 m_nType;
		uint16 m_nStride;
		uint8 m_nDepth;
		pad<0x1> field_F;
		uint8* m_pPixelData;
		pad<0x8> field_18;
		grcImage* m_pNextMipLevel;
		grcImage* m_pNextMajorLevel;
		uint32 m_nRefCount;
		pad<0x2C> field_34;
	};

	class grcTextureFactoryDX11
	{
	public:
		struct TextureCreateParams
		{
			uint32 field_0;
			uint32 field_4;
			uint32 field_8;
			uint32 field_C;
			uint64 field_10;
			uint32 field_18;
			uint32 field_1C;
			uint32 field_20;
			uint32 field_24;
			uint32 field_28;
			uint32 field_2C;
		};
		
		virtual ~grcTextureFactoryDX11() {}
		virtual void f_8() {}
		virtual grcTextureDX11* CreateManualTexture(uint32 width, uint32 height, grcRenderTargetFormat format, void* unk, bool unused, const TextureCreateParams* desc) { return nullptr; }
		virtual grcTextureDX11* CreateImage(grcImage* data, void* unk) { return nullptr; }
		// ...
	};
}