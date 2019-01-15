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
			int16 insertIndex = 0;
			if (m_keys.m_nCount > 0)
			{
				for (int16 i = 0; i < m_keys.m_nCount; i++)
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

			for (int16 i = m_keys.m_nCount; i > insertIndex; --i)
			{
				m_keys.m_pItems[i] = m_keys.m_pItems[i - 1];
			}
			++m_keys.m_nCount;
			m_keys.m_pItems[insertIndex] = key;

			for (int16 i = m_values.m_nCount; i > insertIndex; --i)
			{
				m_values.m_pItems[i] = m_values.m_pItems[i - 1];
			}
			++m_values.m_nCount;
			m_values.m_pItems[insertIndex] = value;
			return true;
		}

		T* Find(atHashValue key)
		{
			int16 index = -1;

			pgDictionary<T>* dict = this;
			while (true)
			{
				int16 left = 0;
				int16 right = dict->m_keys.m_nCount - 1;

				if (right < 0)
				{
					index = -1;
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
							index = -1;
							break;
						}
					}
				}
				if (index != -1)
					return dict->m_values.m_pItems[index];

				dict = dict->m_pParent;
				if (!dict)
					return nullptr;
			}
		}

		bool Remove(atHashValue key)
		{
			int16 index = -1;

			pgDictionary<T>* dict = this;
			while (true)
			{
				int16 left = 0;
				int16 right = dict->m_keys.m_nCount - 1;

				if (right < 0)
				{
					index = -1;
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
							index = -1;
							break;
						}
					}
				}
				
				if (index != -1)
				{
					for (int16 i = index; i < dict->m_keys.m_nCount - 1; i++)
					{
						dict->m_keys.m_pItems[i] = dict->m_keys.m_pItems[i + 1];
					}
					dict->m_keys.m_nCount--;

					for (int16 i = index; i < dict->m_values.m_nCount - 1; i++)
					{
						dict->m_values.m_pItems[i] = dict->m_values.m_pItems[i + 1];
					}
					dict->m_values.m_nCount--;

					return true;
				}

				dict = dict->m_pParent;
				if (!dict)
					return false;
			}
		}

		inline T* GetAt(int16 index)
		{
			return *m_values.Get(index);
		}

		inline int16 GetCount() const
		{
			return m_keys.m_nCount;
		}

		void EnsureSize(int16 min)
		{
			m_keys.EnsureSize(min);
			m_values.EnsureSize(min);
		}
	};
}