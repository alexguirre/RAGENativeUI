#pragma once

namespace rage
{
	uint32 GetHashKey(const char* text, uint32 startHash = 0);
	uint32 GetHashKeyPartial(const char* text, uint32 startHash = 0);
}