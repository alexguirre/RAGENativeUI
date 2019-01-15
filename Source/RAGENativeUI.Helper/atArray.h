#pragma once

#include "sysMemAllocator.h"
#include "sysUseAllocator.h"
#include "Memory.h"

namespace rage
{
	template<typename T>
	class atArray : public sysUseAllocator
	{
	public:
		T * m_pItems;
		int16 m_nCount;
		int16 m_nSize;
		pad<0x4> field_C;

		atArray() {}

		atArray(int16 size)
		{
			uint32 bytesCount = size * sizeof(T);
			m_pItems = (T*)Memory::ms_pAllocator->Allocate(bytesCount);
			memset(m_pItems, 0, bytesCount);

			m_nSize = size;
			m_nCount = 0;
		}

		~atArray()
		{
			if (m_pItems)
			{
				Memory::ms_pAllocator->Free(m_pItems);
			}
			m_nSize = 0;
			m_nCount = 0;
		}

		T* Get(int16 index)
		{
			if (index >= m_nCount)
			{
				return nullptr;
			}

			return &m_pItems[index];
		}

		void Set(int16 index, const T& value)
		{
			if (index >= m_nCount)
			{
				return;
			}

			m_pItems[index] = value;
		}

		void Add(const T& value)
		{
			if (m_nCount == m_nSize)
			{
				EnsureSize(m_nSize + 1);
			}

			m_pItems[m_nCount] = value;
			m_nCount++;
		}

		void Remove(int16 index)
		{
			if (index >= m_nCount)
			{
				return;
			}

			m_nCount--;
			for (; index < m_nCount; index++)
			{
				m_pItems[index] = m_pItems[index + 1];
			}

		}

		void Clear()
		{
			m_nCount = 0;
		}

		void EnsureSize(int16 min)
		{
			if (m_nSize >= min)
			{
				return;
			}

			int16 newSize = m_nSize == 0 ? 8 : m_nSize * 2;
			if (newSize < min)
			{
				newSize = min;
			}

			uint32 bytesCount = (uint32)(newSize * sizeof(T));
			T* newItems = (T*)Memory::ms_pAllocator->Allocate(bytesCount);
			memset(newItems, 0, bytesCount);

			// copy the existing entries
			if (m_pItems)
			{
				memcpy(newItems, m_pItems, m_nCount * sizeof(T));
				Memory::ms_pAllocator->Free(m_pItems);
			}

			m_pItems = newItems;
			m_nSize = newSize;
		}
	};
}