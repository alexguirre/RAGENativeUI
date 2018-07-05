#pragma once

namespace rage
{
	class CStreaming
	{
	public:
		struct DataEntry
		{
			uint32 m_nHandle;
			uint32 m_nFlags;
		};

		DataEntry* m_pEntries;
		uint32 m_nEntriesCount;
		// ...
	};
}