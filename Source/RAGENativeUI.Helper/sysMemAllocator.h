#pragma once

namespace rage
{
	class sysMemAllocator
	{
	public:
		virtual ~sysMemAllocator() = 0;
		virtual void f_8() = 0;
		virtual void* Allocate(int64 size, int64 align, int32 subAllocator) = 0;
		virtual void f_18() = 0;
		virtual void Free(void* pointer) = 0;
		// ...

		inline void* Allocate(int64 size)
		{
			return Allocate(size, 16, 0);
		}
	};
}