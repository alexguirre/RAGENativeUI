#include "Memory.h"
#include "Hooking.Patterns.h"
#include "UsingAllocator.h"
#include <iostream>

bool Memory::ms_bInitialized = false;
rage::sysMemAllocator* Memory::ms_pAllocator = nullptr;
rage::strStreamingModule* Memory::ms_pTxdStore = nullptr;
rage::CStreaming* Memory::ms_pStreaming = nullptr;
rage::grcTextureFactoryDX11* Memory::m_pTextureFactory = nullptr;
rage::pgDictionary<rage::grcTextureDX11>* Memory::ms_pCustomTextureDictionary = nullptr;
std::vector<Memory::CustomTexture> Memory::ms_customTextures;
uint32 Memory::m_nLastCustomTextureId = 0;
void(*Memory::m_pBeginUsingDeviceContext)() = nullptr;
void(*Memory::m_pEndUsingDeviceContext)() = nullptr;
uint32 Memory::m_nTlsAllocatorOffset1 = 0;
uint32 Memory::m_nTlsAllocatorOffset2 = 0;
uint32 Memory::m_nTlsAllocatorOffset3 = 0;

bool Memory::Init()
{
	if(ms_bInitialized)
	{
		return true;
	}

	// TODO: error checking
	uintptr_t address = (uintptr_t)hook::get_pattern("48 8D 1D ? ? ? ? A8 08 75 1D 83 C8 08 48 8B CB 89 05", 3);
	address = address + *(int32*)(address) + 4;
	ms_pAllocator = (rage::sysMemAllocator*)address;

	address = (uintptr_t)hook::get_pattern("48 8D 0D ? ? ? ? C7 44 24 ? ? ? ? ? E8 ? ? ? ? 8B 15", 3);
	address = address + *(int32*)(address) + 4;
	ms_pTxdStore = (rage::strStreamingModule*)address;

	address = (uintptr_t)hook::get_pattern("48 8B 0D ? ? ? ? 41 03 D0 44", 3);
	address = address + *(int32*)(address) + 4;
	ms_pStreaming = (rage::CStreaming*)address;

	address = (uintptr_t)hook::get_pattern("48 8B 0D ? ? ? ? 4C 8B 0D ? ? ? ? 48 8B 01", 3);
	address = address + *(int32*)(address) + 4;
	m_pTextureFactory = *(rage::grcTextureFactoryDX11**)address;

	address = (uintptr_t)hook::get_pattern("65 48 8B 04 25 ? ? ? ? 8B 1D ? ? ? ? 41 B8 ? ? ? ? 48 8B 14 D8", -6);
	m_pBeginUsingDeviceContext = (void(*)())address;

	address = (uintptr_t)hook::get_pattern("40 53 48 83 EC 20 8B 1D ? ? ? ? 65 48 8B 04 25");
	m_pEndUsingDeviceContext = (void(*)())address;

	address = (uintptr_t)hook::get_pattern("48 8B 14 C8 B8 ? ? ? ? 48 89 1C 10", 23);
	m_nTlsAllocatorOffset1 = *(uint32*)address;

	address = (uintptr_t)hook::get_pattern("48 8B 14 C8 B8 ? ? ? ? 48 89 1C 10", 14);
	m_nTlsAllocatorOffset2 = *(uint32*)address;

	address = (uintptr_t)hook::get_pattern("48 8B 14 C8 B8 ? ? ? ? 48 89 1C 10", 5);
	m_nTlsAllocatorOffset3 = *(uint32*)address;

	ms_bInitialized = true;
	return true;
}

bool Memory::DoesTextureDictionaryExist(const char* name) 
{
	uint32 id = 0xFFFFFFFF;
	ms_pTxdStore->GetIdByName(&id, name);
	return id != 0xFFFFFFFF;
}

bool Memory::DoesCustomTextureExist(const char* name) 
{
	if(!ms_pCustomTextureDictionary)
	{
		return false;
	}

	rage::grcTextureDX11* tex = ms_pCustomTextureDictionary->Find(name);
	return tex != nullptr;
}

uint32 Memory::CreateCustomTexture(const char* name, uint32 width, uint32 height, uint8* pixelData, bool updatable)
{
	UsingAllocator usingTlsAllocator;

	if(DoesCustomTextureExist(name))
	{
		return 0xFFFFFFFF;
	}

	if(!ms_pCustomTextureDictionary)
	{
		CreateCustomTextureDictionary();
	}

	rage::grcTextureDX11* tex = nullptr;

	BeginUsingDeviceContext();
	if(updatable)
	{
		rage::grcManualTextureDesc desc;
		memset(&desc, 0, sizeof(desc));
		desc.field_0 = 0;
		desc.field_4 = 0;
		desc.field_10 = 0;
		desc.field_1C = 0;
		desc.field_28 = 0;
		desc.field_8 = 3;
		desc.field_18 = 1;
		desc.field_20 = 2;
		desc.field_24 = 1;

		tex = m_pTextureFactory->CreateManualTexture(width, height, rage::grcRenderTargetFormat::B8G8R8A8_UNORM, nullptr, true, &desc);
		tex->m_pName = CloneString(name);

		if(pixelData)
		{
			rage::grcMappedTexture mappedTex;
			if (tex->Map(0, 0, &mappedTex, rage::grcMapFlags::Write))
			{
				memcpy(mappedTex.m_pData, pixelData, mappedTex.m_nPitch * mappedTex.m_nHeight);
				tex->Unmap(&mappedTex);
			}
		}
		else
		{
			rage::grcMappedTexture mappedTex;
			if (tex->Map(0, 0, &mappedTex, rage::grcMapFlags::Write))
			{
				memset(mappedTex.m_pData, 0xFF, mappedTex.m_nPitch * mappedTex.m_nHeight);
				tex->Unmap(&mappedTex);
			}
		}
	}
	else
	{
		rage::grcTextureData data;
		memset(&data, 0, sizeof(data));
		data.m_nWidth = width;
		data.m_nHeight = height;
		data.m_nDepth = 1;
		data.m_nStride = width * 4;
		data.m_nFormat = rage::grcPixelFormat::B8G8R8A8_UNORM;
		data.m_pPixelData = (uint8*)pixelData;

		tex = m_pTextureFactory->CreateImage(&data, nullptr);
		ms_pAllocator->Free((void*)tex->m_pName); // m_pName already set to "image" by CreateImage
		tex->m_pName = CloneString(name);
	}
	EndUsingDeviceContext();
	std::cout << "No longer using D3D Device Context\n";

	ms_pCustomTextureDictionary->Add(name, tex);
	std::cout << "Added texture to custom dictionary\n";

	std::cout << "Texture Id -> " << m_nLastCustomTextureId << "\n";
	CustomTexture customTex{ m_nLastCustomTextureId, updatable, tex };
	m_nLastCustomTextureId++;
	ms_customTextures.push_back(customTex);

	return customTex.m_nId;
}

uint32 Memory::GetNumberOfTexturesFromDictionary(const char* name) 
{
	uint32 id = 0xFFFFFFFF;
	ms_pTxdStore->GetIdByName(&id, name);

	if (id != 0xFFFFFFFF)
	{
		rage::pgDictionary<rage::grcTextureDX11>* txd = (rage::pgDictionary<rage::grcTextureDX11>*)ms_pTxdStore->GetAsset(id);
		if(txd)
		{
			return txd->GetCount();
		}
	}
	return 0xFFFFFFFF;
}

void Memory::GetTexturesFromDictionary(const char* name, TextureDesc* outTextureDescs)
{
	uint32 id = 0xFFFFFFFF;
	ms_pTxdStore->GetIdByName(&id, name);

	if (id != 0xFFFFFFFF)
	{
		rage::pgDictionary<rage::grcTextureDX11>* txd = (rage::pgDictionary<rage::grcTextureDX11>*)ms_pTxdStore->GetAsset(id);
		if (txd)
		{
			uint16 c = txd->GetCount();
			if(c > 0)
			{
				for (uint16 i = 0; i < c; i++)
				{
					rage::grcTextureDX11* tex = txd->GetAt(i);
					outTextureDescs[i].m_pName = tex->m_pName;
					outTextureDescs[i].m_nWidth = tex->m_nWidth;
					outTextureDescs[i].m_nHeight = tex->m_nHeight;
				}
			}
		}
	}

}

void Memory::CreateCustomTextureDictionary()
{
	constexpr const char* Name = "rnui_custom_textures";

	uint32 id = 0xFFFFFFFF;
	ms_pTxdStore->GetIdByName(&id, Name);

	if (id != 0xFFFFFFFF)
	{
		if (!ms_pCustomTextureDictionary)
		{
			ms_pCustomTextureDictionary = (rage::pgDictionary<rage::grcTextureDX11>*)ms_pTxdStore->GetAsset(id);
		}
		return;
	}


	ms_pTxdStore->GetOrCreate(&id, Name);

	if (id != 0xFFFFFFFF)
	{
		rage::CStreaming::DataEntry& entry = ms_pStreaming->m_pEntries[ms_pTxdStore->m_nBaseIndex + id];

		if (!entry.m_nHandle)
		{
			rage::pgDictionary<rage::grcTextureDX11>* txd = new rage::pgDictionary<rage::grcTextureDX11>();

			rage::strAssetReference ref;
			ref.m_pAsset = txd;

			ms_pTxdStore->SetAssetReference(id, ref);
			entry.m_nFlags = (512 << 8) | 1;

			ms_pTxdStore->AddRef(id);

			ms_pCustomTextureDictionary = txd;
		}
	}
	else
	{
		ms_pCustomTextureDictionary = nullptr;
	}
}

void Memory::BeginUsingDeviceContext() 
{
	m_pBeginUsingDeviceContext();
}

void Memory::EndUsingDeviceContext() 
{
	m_pEndUsingDeviceContext();
}

char* Memory::CloneString(const char* src)
{
	if (!src)
		return nullptr;

	size_t l = strlen(src) + 1;
	char* dst = (char*)ms_pAllocator->Allocate(l);
	memmove(dst, src, l);
	return dst;
}