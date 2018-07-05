#pragma once
#include "sysUseAllocator.h"
#include "atHashValue.h"
#include "atArray.h"

#include <iostream>

namespace rage
{
	template<typename T>
	class pgDictionary : public sysUseAllocator
	{
	public:
		void* m_pBlockMap;
		pgDictionary<T>* m_pParent;
		uint32 m_nUsageCount;
		pad<0x4> field_1C;
		atArray<atHashValue> m_keys;
		atArray<T*> m_values;

		pgDictionary()
		{
			m_nUsageCount = 1;
			m_pParent = nullptr;
		}

		pgDictionary(uint16 size)
		{
			m_nUsageCount = 1;
			m_pParent = nullptr;
			m_keys = atArray<atHashValue>(size);
			m_values = atArray<T*>(size);
		}

		virtual ~pgDictionary() {}
		virtual void field_8() {}
		virtual void field_10() {}

		bool Add(atHashValue key, T* value)
		{
			uint16 insertIndex = 0;
			if (m_keys.m_nCount > 0)
			{
				for (uint16 i = 0; i < m_keys.m_nCount; i++)
				{
					if (m_keys.m_pItems[i] == key)
					{
						return false;
					}

					if (m_keys.m_pItems[i] <= key)
					{
						insertIndex++;
					}
				}
			}

			if (insertIndex == m_keys.m_nSize)
			{
				EnsureSize(m_keys.m_nSize ? m_keys.m_nSize * 2 : 5);
			}

			for (uint16 i = m_keys.m_nCount; i > insertIndex; --i)
			{
				m_keys.m_pItems[i] = m_keys.m_pItems[i - 1];
			}
			++m_keys.m_nCount;
			m_keys.m_pItems[insertIndex] = key;

			for (uint16 i = m_values.m_nCount; i > insertIndex; --i)
			{
				m_values.m_pItems[i] = m_values.m_pItems[i - 1];
			}
			++m_values.m_nCount;
			m_values.m_pItems[insertIndex] = value;
			return true;
		}

		T* Find(atHashValue key)
		{
			uint16 index = 0xFFFF;

			pgDictionary<T>* dict = this;
			while (true)
			{
				uint16 left = 0;
				uint16 right = dict->m_keys.m_nCount - 1;

				if (right < 0)
				{
					index = 0xFFFF;
				}
				else
				{
					while (true)
					{
						index = ((right + left) >> 1);
						if (key == dict->m_keys.m_pItems[index])
							break;
						if (key >= dict->m_keys.m_pItems[index])
							left = index + 1;
						else
							right = index - 1;
						if (left > right)
						{
							index = 0xFFFF;
							break;
						}
					}
				}
				if (index != 0xFFFF)
					return dict->m_values.m_pItems[index];

				dict = dict->m_pParent;
				if (!dict)
					return nullptr;
			}
		}

		inline T* GetAt(uint16 index)
		{
			return *m_values.Get(index);
		}

		inline uint16 GetCount() const
		{
			return m_keys.m_nCount;
		}

		void EnsureSize(uint16 min)
		{
			m_keys.EnsureSize(min);
			m_values.EnsureSize(min);
		}
	};
}