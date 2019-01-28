#pragma once
#include "sysUseAllocator.h"
#include <dxgiformat.h>

namespace rage
{
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

	struct grcPoint
	{
		int X, Y;
	};

	struct grcRect
	{
		int Left, Top, Right, Bottom;
	};

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
		virtual void f_1() {}
		virtual void f_2() {}
		virtual void f_3() {}
		virtual void f_4() {}
		virtual void f_5() {}
		virtual void f_6() {}
		virtual void f_7() {}
		virtual void f_8() {}
		virtual void f_9() {}
		virtual void f_10() {}
		virtual void f_11() {}
		virtual void f_12() {}
		virtual void f_13() {}
		virtual void f_14() {}
		virtual void f_15() {}
		virtual void f_16() {}
		virtual void f_17() {}
		virtual void f_18() {}
		virtual void f_19() {}
		virtual void f_20() {}
		virtual void f_21() {}
		virtual void f_22() {}
		virtual void f_23() {}
		virtual void f_24() {}
		virtual bool Map(uint32 subLevelsCount, uint32 subLevel, grcMappedTexture *outMappedTexture, grcMapFlags flags) { return false; }
		virtual void Unmap(grcMappedTexture *mappedTexture) {}
		virtual void f_27() {}
		virtual void f_28() {}
		virtual void f_29() {}
		virtual void f_30() {}
		virtual void f_31() {}
		virtual void f_32() {}
		virtual void f_33() {}
		virtual void f_34() {}
		virtual void f_35() {}
		// srcLastPixel: x == rowPitch and y == height
		virtual bool Copy2D(const void* srcData, const grcPoint& srcLastPixel, const grcRect& dstRect, const /*grcTextureLock*/void* _unused, int subResource) { return false; }
		// ...
	};
}