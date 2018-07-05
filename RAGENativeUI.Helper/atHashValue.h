#pragma once

#include "Hashing.h"

namespace rage
{
	class atHashValue
	{
	public:
		uint32 m_nValue;

		inline atHashValue()
		{
			m_nValue = 0;
		}

		inline atHashValue(const char* value)
		{
			m_nValue = rage::GetHashKey(value);
		}

		inline atHashValue(uint32 value)
		{
			m_nValue = value;
		}

		inline operator uint32() const
		{
			return m_nValue;
		}

		inline bool operator==(atHashValue other) const
		{
			return m_nValue == other.m_nValue;
		}

		inline bool operator!=(atHashValue other) const
		{
			return m_nValue != other.m_nValue;
		}

		inline bool operator==(uint32 other) const
		{
			return m_nValue == other;
		}

		inline bool operator!=(uint32 other) const
		{
			return m_nValue != other;
		}
	};
}