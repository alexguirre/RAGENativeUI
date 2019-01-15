#pragma once
#include "sysMemAllocator.h"
#include "strStreamingModule.h"
#include "CStreaming.h"
#include "pgDictionary.h"
#include "grcTextureDX11.h"
#include "grcTextureFactoryDX11.h"
#include <unordered_map>

class Memory
{
public:
	struct CustomTexture 
	{
		rage::atHashValue m_nName;
		bool m_bUpdatable;
		rage::grcTextureDX11* m_pTexture;
	};

	struct TextureDesc
	{
		const char* m_pName;
		uint32 m_nWidth;
		uint32 m_nHeight;
	};

	struct CustomTextureDesc
	{
		const char* m_pName;
		uint32 m_nWidth;
		uint32 m_nHeight;
		uint32 m_nNameHash;
		bool m_bUpdatable;
	};

	static bool ms_bInitialized;
	static rage::sysMemAllocator* ms_pAllocator;
	static rage::strStreamingModule* ms_pTxdStore;
	static rage::CStreaming* ms_pStreaming;
	static rage::grcTextureFactoryDX11* m_pTextureFactory;
	static rage::pgDictionary<rage::grcTextureDX11>* ms_pCustomTextureDictionary;
	static std::unordered_map<uint32, CustomTexture> ms_customTextures;
	static uint32 m_nTlsAllocatorOffset1;
	static uint32 m_nTlsAllocatorOffset2;
	static uint32 m_nTlsAllocatorOffset3;

	static bool Init();

	static bool DoesTextureDictionaryExist(const char* name);
	static uint32 GetNumberOfTexturesFromDictionary(const char* name);
	static void GetTexturesFromDictionary(const char* name, TextureDesc* outTextureDescs);
	static bool DoesCustomTextureExist(rage::atHashValue name);
	static bool CreateCustomTexture(const char* name, uint32 width, uint32 height, uint8* pixelData, bool updatable);
	static void DeleteCustomTexture(rage::atHashValue name);
	static uint32 GetNumberOfCustomTextures();
	static void GetCustomTextures(Memory::CustomTextureDesc* outTextureDescs);
private:
	static void(*m_pBeginUsingDeviceContext)();
	static void(*m_pEndUsingDeviceContext)();

	static void CreateCustomTextureDictionary();
	static void BeginUsingDeviceContext();
	static void EndUsingDeviceContext();
	static char* CloneString(const char* string);

	Memory() {}
};
//struct CustomTextureDesc
//{
//	const char* m_pName;
//	uint32 m_nWidth;
//	uint32 m_nHeight;
//	uint32 m_nNameHash;
//	bool m_bUpdatable;
//};
//int i = sizeof(Memory::CustomTextureDesc);
//int i = sizeof(Memory::CustomTextureDesc);
//int i = sizeof(Memory::CustomTextureDesc);
//int i = sizeof(Memory::CustomTextureDesc);