#include "UsingAllocator.h"
#include "Memory.h"

UsingAllocator::UsingAllocator()
{
	char* tls = *(char**)__readgsqword(0x58);
	m_old1 = *(void**)(tls + Memory::m_nTlsAllocatorOffset1);
	m_old2 = *(void**)(tls + Memory::m_nTlsAllocatorOffset2);
	m_old3 = *(void**)(tls + Memory::m_nTlsAllocatorOffset3);

	*(void**)(tls + Memory::m_nTlsAllocatorOffset1) = Memory::ms_pAllocator;
	*(void**)(tls + Memory::m_nTlsAllocatorOffset2) = Memory::ms_pAllocator;
	*(void**)(tls + Memory::m_nTlsAllocatorOffset3) = Memory::ms_pAllocator;
}


UsingAllocator::~UsingAllocator()
{
	char* tls = *(char**)__readgsqword(0x58);
	*(void**)(tls + Memory::m_nTlsAllocatorOffset1) = m_old1;
	*(void**)(tls + Memory::m_nTlsAllocatorOffset2) = m_old2;
	*(void**)(tls + Memory::m_nTlsAllocatorOffset3) = m_old3;
}
