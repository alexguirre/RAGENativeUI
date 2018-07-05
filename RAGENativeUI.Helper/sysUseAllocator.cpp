#include "sysUseAllocator.h"
#include "Memory.h"

namespace rage
{
	void* sysUseAllocator::operator new(size_t size)
	{
		return Memory::ms_pAllocator->Allocate(size);
	}

	void* sysUseAllocator::operator new[](size_t size)
	{
		return Memory::ms_pAllocator->Allocate(size);
	}

	void sysUseAllocator::operator delete(void* ptr)
	{
		Memory::ms_pAllocator->Free(ptr);
	}

	void sysUseAllocator::operator delete[](void* ptr)
	{
		Memory::ms_pAllocator->Free(ptr);
	}
}