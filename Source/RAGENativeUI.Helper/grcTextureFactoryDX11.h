#pragma once

#include "grcTextureDX11.h"

namespace rage
{
	struct grcTextureData
	{
		uint16 m_nWidth;
		uint16 m_nHeight;
		grcPixelFormat m_nFormat;
		uint32 m_nType;
		uint16 m_nStride;
		uint8 m_nDepth;
		pad<0x1> field_F;
		uint8* m_pPixelData;
		pad<0x8> field_18;
		grcTextureData* m_pNextMipLevel;
		grcTextureData* m_pNextMajorLevel;
		uint32 m_nRefCount;
		pad<0x2C> field_34;
	};

	struct grcManualTextureDesc
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

	class grcTextureFactoryDX11
	{
	public:
		virtual ~grcTextureFactoryDX11() {}
		virtual void f_8() {}
		virtual grcTextureDX11* CreateManualTexture(uint32 width, uint32 height, grcRenderTargetFormat format, void* unk, bool unused, const grcManualTextureDesc* desc) { return nullptr; }
		virtual grcTextureDX11* CreateImage(grcTextureData* data, void* unk) { return nullptr; }
		// ...
	};
}