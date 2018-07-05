#pragma once

namespace rage
{
	struct strAssetReference
	{
		pad<0x8> field_0;
		void* m_pAsset;
	};


	class strStreamingModule 
	{
	public:
		uint32 m_nBaseIndex;
		// ...

		virtual ~strStreamingModule() {}
		virtual uint32* GetOrCreate(uint32* id, const char* name) { return nullptr; }
		virtual uint32* GetIdByName(uint32* outId, const char* name) { return nullptr; }
		virtual void f_18() {}
		virtual void f_20() {}
		virtual void f_28() {}
		virtual void f_30() {}
		virtual void SetAssetReference(uint32 id, strAssetReference& reference) {}
		virtual void* GetAsset(uint32 id) { return nullptr; }
		virtual void f_48() {}
		virtual void f_50() {}
		virtual void f_58() {}
		virtual void f_60() {}
		virtual void f_68() {}
		virtual void f_70() {}
		virtual void f_78() {}
		virtual void AddRef(uint32 id) {}
		// ...
	};
}