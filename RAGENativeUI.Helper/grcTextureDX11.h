#pragma once
#include "sysUseAllocator.h"
#include <dxgiformat.h>

namespace rage
{
	enum class grcPixelFormat : uint32
	{
		B8G8R8A8_UNORM = 11,
	};

	enum class grcRenderTargetFormat : uint32
	{
		B8G8R8A8_UNORM = 2,
	};

	struct grcMappedTexture
	{
		uint32 m_nSubLevel;
		pad<0x4> field_4;
		uint8* m_pData;
		uint32 m_nPitch;
		pad<0x4> field_14;
		uint32 m_nWidth;
		uint32 m_nHeight;
		DXGI_FORMAT m_nFormat;
		uint32 m_nSubLevelsCount;
	};

	enum class grcMapFlags : uint32
	{
		Read = 0x1,
		Write = 0x2,
		Unknown = 0x4,
		WriteDiscard = 0x8,
		NoOverwrite = 0x10
	};
	DEFINE_ENUM_FLAG_OPERATORS(grcMapFlags);

	class grcTextureDX11 : public sysUseAllocator
	{
	public:
		pad<0x20> field_8;
		const char* m_pName;
		pad<0x20> field_30;
		uint16 m_nWidth;
		uint16 m_nHeight;
		// ...

		virtual ~grcTextureDX11() {}
		virtual void f_8() {}
		virtual void f_10() {}
		virtual void f_18() {}
		virtual void f_20() {}
		virtual void f_28() {}
		virtual void f_30() {}
		virtual void f_38() {}
		virtual void f_40() {}
		virtual void f_48() {}
		virtual void f_50() {}
		virtual void f_58() {}
		virtual void f_60() {}
		virtual void f_68() {}
		virtual void f_70() {}
		virtual void f_78() {}
		virtual void f_80() {}
		virtual void f_88() {}
		virtual void f_90() {}
		virtual void f_98() {}
		virtual void f_A0() {}
		virtual void f_A8() {}
		virtual void f_B0() {}
		virtual void f_B8() {}
		virtual void f_C0() {}
		virtual bool Map(uint32 subLevelsCount, uint32 subLevel, grcMappedTexture *outMappedTexture, grcMapFlags flags) { return false; }
		virtual void Unmap(grcMappedTexture *mappedTexture) {}
		// ...
	};
}