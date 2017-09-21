namespace RAGENativeUI
{
    using System;
    using System.Linq;
    using System.Diagnostics;
    using System.Collections;
    using System.Collections.Generic;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Memory;

    // defined in timecycle_mods_*.xml
    /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Doc/*' />
    public unsafe sealed class TimeCycleModifier : IAddressable
    {
        private uint hash;
        private IntPtr memAddress;
        private int index = -1;

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="Hash"]/*' />
        public uint Hash { get { return hash; } }
        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="Name"]/*' />
        public string Name
        {
            get
            {
                if (knownNames.TryGetValue(hash, out string n))
                {
                    return n;
                }

                return $"0x{hash.ToString("X")}";
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="IsActive"]/*' />
        public bool IsActive
        {
            get
            {
                return *GameMemory.CurrentTimeCycleModifierIndex == index;
            }
            set
            {
                if (value)
                {
                    CurrentModifier = this;
                }
                else if (IsActive)
                {
                    CurrentModifier = null;
                }
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="MemoryAddress"]/*' />
        public IntPtr MemoryAddress { get { return memAddress; } }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="Index"]/*' />
        public int Index { get { return index; } }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="Flags"]/*' />
        public uint Flags { get { return ((CTimeCycleModifier*)memAddress)->Flags; } }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="Mods"]/*' />
        public TimeCycleModifierModsCollection Mods { get; }

        // from existing timecycle modifier in memory
        private TimeCycleModifier(CTimeCycleModifier* native, int idx)
        {
            hash = native->Name;
            memAddress = (IntPtr)native;
            index = idx;
            Mods = new TimeCycleModifierModsCollection(this);
        }

        // creates new timecycle modifier in memory
        private TimeCycleModifier(string name, uint flags, CTimeCycleModifier.Mod[] mods)
        {
            uint hash = Game.GetHashKey(name);
            knownNames[hash] = name;
            if (GameMemory.TimeCycleModifiersManager->IsNameUsed(hash))
                throw new InvalidOperationException($"The name '{name}' is already in use.");


            this.hash = hash;
            memAddress = (IntPtr)GameMemory.TimeCycleModifiersManager->NewTimeCycleModifier(hash, mods, flags);
            index = GameMemory.TimeCycleModifiersManager->Modifiers.Count - 1;
            Mods = new TimeCycleModifierModsCollection(this);
            cache[hash] = this;
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="Ctor1"]/*' />
        public TimeCycleModifier(string name, TimeCycleModifier template)
            : this(name, template.Flags, template.Mods.Select(m => new CTimeCycleModifier.Mod { ModType = (int)m.Type, Value1 = m.Value1, Value2 = m.Value2 }).ToArray())
        {
        }

        // TODO: maybe change Tuple<TimeCycleModifierModType, float, float> to a custom struct
        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="Ctor2"]/*' />
        public TimeCycleModifier(string name, uint flags, params Tuple<TimeCycleModifierModType, float, float>[] mods)
            : this(name, flags, mods.Select(m => new CTimeCycleModifier.Mod { ModType = (int)m.Item1, Value1 = m.Item2, Value2 = m.Item3 }).ToArray())
        {
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="IsValid"]/*' />
        public bool IsValid()
        {
            return memAddress != IntPtr.Zero;
        }



        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="GetByName"]/*' />
        public static TimeCycleModifier GetByName(string name)
        {
            uint hash = Game.GetHashKey(name);
            knownNames[hash] = name;
            return GetByHash(hash);
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="GetByHash"]/*' />
        public static TimeCycleModifier GetByHash(uint hash)
        {
            if (cache.TryGetValue(hash, out TimeCycleModifier p))
            {
                return p;
            }
            else
            {
                int index= GameFunctions.GetTimeCycleModifierIndex(GameMemory.TimeCycleModifiersManager, &hash);

                if (index != -1)
                {
                    return GetByIndex(index);
                }
            }

            return null;
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="GetByIndex"]/*' />
        public static TimeCycleModifier GetByIndex(int index)
        {
            if (index < 0 || index >= NumberOfTimeCycleModifiers)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            short i = (short)index;
            CTimeCycleModifier* native = GameMemory.TimeCycleModifiersManager->Modifiers.Get(i);

            if (native != null)
            {
                if (cache.TryGetValue(native->Name, out TimeCycleModifier p))
                {
                    return p;
                }
                else
                {
                    TimeCycleModifier m = new TimeCycleModifier(native, index);
                    cache[native->Name] = m;
                    return m;
                }
            }

            return null;
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="GetAll"]/*' />
        public static TimeCycleModifier[] GetAll()
        {
            TimeCycleModifier[] mods = new TimeCycleModifier[GameMemory.TimeCycleModifiersManager->Modifiers.Count];
            for (short i = 0; i < GameMemory.TimeCycleModifiersManager->Modifiers.Count; i++)
            {
                mods[i] = GetByIndex(i);
            }

            return mods;
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="NumberOfTimeCycleModifiers"]/*' />
        public static int NumberOfTimeCycleModifiers
        {
            get
            {
                return GameMemory.TimeCycleModifiersManager->Modifiers.Count;
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="CurrentModifier"]/*' />
        public static TimeCycleModifier CurrentModifier
        {
            get
            {
                int index = *GameMemory.CurrentTimeCycleModifierIndex;
                return index == -1 ? null : GetByIndex(index);
            }
            set
            {
                if(value == null || !value.IsValid())
                {
                    *GameMemory.CurrentTimeCycleModifierIndex = -1;
                    // set strength to 1.0 to have the same behaviour as the CLEAR_TIMECYCLE_MODIFIER native
                    *GameMemory.CurrentTimeCycleModifierStrength = 1.0f;
                }
                else
                {
                    *GameMemory.CurrentTimeCycleModifierIndex = value.Index;
                }
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="CurrentModifierStrength"]/*' />
        public static float CurrentModifierStrength
        {
            get
            {
                return *GameMemory.CurrentTimeCycleModifierStrength;
            }
            set
            {
                *GameMemory.CurrentTimeCycleModifierStrength = value;
            }
        }

        private static Dictionary<uint, TimeCycleModifier> cache = new Dictionary<uint, TimeCycleModifier>();

        private static Dictionary<uint, string> knownNames = new Dictionary<uint, string>()
        {
            { 0x9424AE5C, "li" },
            { 0xA2663418, "underwater" },
            { 0x8EB53546, "underwater_deep" },
            { 0x9CFCA62E, "NoAmbientmult" },
            { 0x298BF805, "superDARK" },
            { 0x35E3B679, "CAMERA_BW" },
            { 0x0DD3AB25, "Forest" },
            { 0xB224DDF2, "micheal" },
            { 0x2737D5AC, "TREVOR" },
            { 0x44C24694, "FRANKLIN" },
            { 0xA2E41EBA, "Tunnel" },
            { 0xF5D822D2, "carpark" },
            { 0xBD2EC26B, "NEW_abattoir" },
            { 0xB9C828C0, "Vagos" },
            { 0xEA4D4BFF, "cops" },
            { 0x12C7AB99, "Bikers" },
            { 0x54A03B2A, "BikersSPLASH" },
            { 0x9BFF93E6, "VagosSPLASH" },
            { 0x256503F0, "CopsSPLASH" },
            { 0xC1BBB5E6, "VAGOS_new_garage" },
            { 0x6BE54AAB, "VAGOS_new_hangout" },
            { 0x86397BFF, "NEW_jewel" },
            { 0xE5F013C9, "frankilnsAUNTS_new" },
            { 0x5701BAEE, "frankilnsAUNTS_SUNdir" },
            { 0xA502C509, "StreetLighting" },
            { 0x71431056, "NEW_tunnels" },
            { 0x5B67DDD1, "NEW_yellowtunnels" },
            { 0xC4A0601B, "NEW_tunnels_hole" },
            { 0x2C540632, "NEW_tunnels_ditch" },
            { 0x09DD4C01, "Paleto" },
            { 0x90B957B8, "new_bank" },
            { 0xEF67FB79, "ReduceDrawDistance" },
            { 0xF6C64A66, "ReduceDrawDistanceMission" },
            { 0xB563551D, "lightpolution" },
            { 0x39CA6D0D, "NEW_lesters" },
            { 0xC0982281, "ReduceDrawDistanceMAP" },
            { 0x79858AFD, "reducewaterREF" },
            { 0x59A64A27, "garage" },
            { 0xA4C0FE80, "LightPollutionHills" },
            { 0x6FC8F456, "NewMicheal" },
            { 0x47D9D70B, "NewMichealupstairs" },
            { 0xBE4BCCF9, "NewMichealstoilet" },
            { 0x7192F554, "NewMichealgirly" },
            { 0x30890C49, "WATER_port" },
            { 0x5B69BBF4, "WATER_salton" },
            { 0x38B8A4BB, "WATER_river" },
            { 0xA7F006C5, "FIB_interview" },
            { 0x5E6B2381, "NEW_station_unfinished" },
            { 0xD41322D6, "cashdepot" },
            { 0xA90155E6, "cashdepotEMERGENCY" },
            { 0xC085C71A, "FrankilinsHOUSEhills" },
            { 0x33578840, "HicksbarNEW" },
            { 0x49E7D0F0, "NOdirectLight" },
            { 0x0545A11D, "SALTONSEA" },
            { 0xA0AC5AD2, "TUNNEL_green" },
            { 0xC4329C49, "NewMicheal_night" },
            { 0x9A88DC11, "WATER_muddy" },
            { 0x431ED452, "WATER_shore" },
            { 0x431375E1, "damage" },
            { 0x039FE203, "hitped" },
            { 0x02EB6E84, "dying" },
            { 0xC1C270AE, "overwater" },
            { 0x9BC6DF8C, "whitenightlighting" },
            { 0xBB1672AC, "TUNNEL_yellow" },
            { 0xDE969E50, "buildingTOP" },
            { 0xFA1010E7, "WATER_lab" },
            { 0x67E6400F, "cinema" },
            { 0x4C1EE91B, "fireDEPT" },
            { 0xA0FAEE7D, "ranch" },
            { 0x1D8DB72F, "TUNNEL_white" },
            { 0x567EC2FB, "V_recycle_mainroom" },
            { 0x42436D9A, "V_recycle_dark" },
            { 0x7F9C0883, "V_recycle_light" },
            { 0x5D9FC746, "lightning_weak" },
            { 0x6FAE7E53, "lightning_strong" },
            { 0xE14C1C19, "lightning_cloud" },
            { 0xE0A171BD, "gunclubrange" },
            { 0xF9512FA6, "NoAmbientmult_interior" },
            { 0xE6AE4E67, "FullAmbientmult_interior" },
            { 0x2D909E01, "StreetLightingJunction" },
            { 0x77F1BF19, "StreetLightingtraffic" },
            { 0xB848D45D, "Multipayer_spectatorCam" },
            { 0x776CC474, "INT_NoAmbientmult" },
            { 0xD40D2976, "INT_NoAmbientmult_art" },
            { 0xDB930C8A, "INT_FullAmbientmult" },
            { 0xD19DCD79, "INT_FULLAmbientmult_art" },
            { 0xF2B2EC73, "INT_FULLAmbientmult_both" },
            { 0x30B01572, "INT_NoAmbientmult_both" },
            { 0x94638209, "Sniper" },
            { 0x38F689F4, "ReduceSSAO" },
            { 0x3B8158DA, "scope_zoom_in" },
            { 0x1FFE7D50, "scope_zoom_out" },
            { 0x9E78A818, "crane_cam" },
            { 0x2BD78DF9, "WATER_silty" },
            { 0x459FAA93, "Trevors_room" },
            { 0xF3CF24C1, "Hint_cam" },
            { 0xC2FB4285, "venice_canal_tunnel" },
            { 0xFE313221, "blackNwhite" },
            { 0x6C2728F7, "projector" },
            { 0xE5AD884C, "paleto_opt" },
            { 0x2609278B, "warehouse" },
            { 0x9957219F, "pulse" },
            { 0xCB89BF1C, "sleeping" },
            { 0xE14D50C8, "INT_garage" },
            { 0x254E1B2C, "nextgen" },
            { 0x3DDFC2C8, "crane_cam_cinematic" },
            { 0xCBCCF9E3, "TUNNEL_orange" },
            { 0xE85406B4, "traffic_skycam" },
            { 0x3F7F466D, "powerstation" },
            { 0x899CCD1C, "SAWMILL" },
            { 0x878ABDF9, "LODmult_global_reduce" },
            { 0x657C748A, "LODmult_HD_orphan_reduce" },
            { 0x30DACF26, "LODmult_HD_orphan_LOD_reduce" },
            { 0x1EECC192, "LODmult_LOD_reduce" },
            { 0x46373B8A, "LODmult_SLOD1_reduce" },
            { 0x3D6C0864, "LODmult_SLOD2_reduce" },
            { 0x2A219604, "LODmult_SLOD3_reduce" },
            { 0xACFD0CA3, "NewMicheal_upstairs" },
            { 0x869CD7A7, "micheals_lightsOFF" },
            { 0xFCB3CFDB, "telescope" },
            { 0x4C80A282, "WATER_silverlake" },
            { 0xE2C27D50, "WATER _lab_cooling" },
            { 0xFB29CD3D, "baseTONEMAPPING" },
            { 0x7AD589DB, "WATER_salton_bottom" },
            { 0x9688BCFB, "new_stripper_changing" },
            { 0xC88CAB7F, "underwater_deep_clear" },
            { 0x5B6BB1E5, "prologue_ending_fog" },
            { 0x10AE73FD, "graveyard_shootout" },
            { 0x0CE3A0AE, "morebloom" },
            { 0xD7845D32, "LIGHTSreduceFALLOFF" },
            { 0xD52FC310, "INT_posh_hairdresser" },
            { 0x7FAED6E6, "V_strip_office" },
            { 0x686DAA86, "sunglasses" },
            { 0x99F93FD5, "vespucci_garage" },
            { 0x8984E781, "half_direct" },
            { 0xC8C2560B, "carpark_dt1_03" },
            { 0x7E78C451, "tunnel_id1_11" },
            { 0xAD26EF06, "reducelightingcost" },
            { 0x2C73A4EA, "NOrain" },
            { 0xF4E103DB, "morgue_dark" },
            { 0xADAC79EB, "CS3_rail_tunnel" },
            { 0x50385919, "new_tunnels_entrance" },
            { 0xFB93942E, "spectator1" },
            { 0x2A5A71BB, "spectator2" },
            { 0xE00EDD25, "spectator3" },
            { 0x10BCBE80, "spectator4" },
            { 0x6697EA35, "spectator5" },
            { 0x5551C7A9, "spectator6" },
            { 0x1B04D310, "spectator7" },
            { 0x37E60CD2, "spectator8" },
            { 0x48D22EBA, "spectator9" },
            { 0x5280F886, "spectator10" },
            { 0xD9512C95, "INT_NOdirectLight" },
            { 0x41B45F20, "WATER_resevoir" },
            { 0x36995838, "WATER_hills" },
            { 0x197C3D4F, "WATER_militaryPOOP" },
            { 0x943B6ED4, "NEW_ornate_bank" },
            { 0xBBD67486, "NEW_ornate_bank_safe" },
            { 0xA15EB144, "NEW_ornate_bank_entrance" },
            { 0x0A789CDA, "NEW_ornate_bank_office" },
            { 0x17DB1899, "LODmult_global_reduce_NOHD" },
            { 0x9FB8A963, "interior_WATER_lighting" },
            { 0x49BB0A33, "gorge_reflectionoffset" },
            { 0x7FC8DBC0, "eyeINtheSKY" },
            { 0x5C1C6E37, "resvoire_reflection" },
            { 0x1D41507D, "NO_weather" },
            { 0x9F0296C1, "prologue_ext_art_amb" },
            { 0xD7F8F07A, "prologue_shootout" },
            { 0xB0B25555, "heathaze" },
            { 0x2FA3373C, "KT_underpass" },
            { 0xB32715EB, "INT_nowaterREF" },
            { 0x0B31112F, "carMOD_underpass" },
            { 0x68CD7004, "refit" },
            { 0x2CF58BCE, "NO_streetAmbient" },
            { 0x64AB86E9, "NO_coronas" },
            { 0xB82ABA2F, "epsilion" },
            { 0x14DD0187, "WATER_refmap_high" },
            { 0xC51BC08B, "WATER_refmap_med" },
            { 0x98403C82, "WATER_refmap_low" },
            { 0xFD8DDA81, "WATER_refmap_verylow" },
            { 0xE85340F3, "WATER_refmap_poolside" },
            { 0x256E2197, "WATER_refmap_silverlake" },
            { 0x8D67F7EE, "WATER_refmap_venice" },
            { 0x40C14593, "FORdoron_delete" },
            { 0xA2344519, "NO_fog_alpha" },
            { 0x84E436AC, "V_strip_nofog" },
            { 0x0774BE43, "METRO_Tunnels" },
            { 0xBA74E943, "METRO_Tunnels_entrance" },
            { 0x0167F859, "METRO_platform" },
            { 0x3BC166C4, "STRIP_stage" },
            { 0x28FC90E5, "STRIP_office" },
            { 0xB40BF03F, "STRIP_changing" },
            { 0xDBACAC5F, "INT_NO_fogALPHA" },
            { 0x17BB693A, "STRIP_nofog" },
            { 0xF1AE1615, "INT_streetlighting" },
            { 0x530EA7C5, "ch2_tunnel_whitelight" },
            { 0x1802A041, "AmbientPUSH" },
            { 0x89CE0D6F, "ship_lighting" },
            { 0x155184C8, "powerplant_nightlight" },
            { 0x4751FD50, "paleto_nightlight" },
            { 0xBAE2E6C4, "militarybase_nightlight" },
            { 0xA7406171, "sandyshore_nightlight" },
            { 0x388E9C03, "jewel_gas" },
            { 0x18667D9A, "WATER_refmap_off" },
            { 0x83B20533, "trailer_explosion_optimise" },
            { 0xC64E40BD, "nervousRON_fog" },
            { 0x1FE397A5, "DONT_overide_sunpos" },
            { 0xAE14E170, "gallery_refmod" },
            { 0x9BBCD4BC, "prison_nightlight" },
            { 0xF84DA7A3, "multiplayer_ped_fight" },
            { 0xD0966739, "ship_explosion_underwater" },
            { 0x5EC0D627, "EXTRA_bouncelight" },
            { 0xF5AE4A07, "secret_camera" },
            { 0xBB373E21, "canyon_mission" },
            { 0xBB79A549, "gorge_reflection_gpu" },
            { 0x9B0EC9DC, "subBASE_water_ref" },
            { 0x83BC5640, "poolsidewaterreflection2" },
            { 0x00EE5A7C, "CUSTOM_streetlight" },
            { 0x18EAD869, "ufo" },
            { 0xD9E2D52B, "lab_none_exit" },
            { 0x253FD4DC, "FinaleBankexit" },
            { 0xFFC4ED39, "prologue_reflection_opt" },
            { 0x0AA23BDC, "tunnel_entrance" },
            { 0x672E5B9B, "tunnel_entrance_INT" },
            { 0x95468C34, "id1_11_tunnel" },
            { 0x72BF3D40, "reflection_correct_ambient" },
            { 0xA583CACC, "scanline_cam_cheap" },
            { 0x340C3607, "scanline_cam" },
            { 0x804510D1, "VC_tunnel_entrance" },
            { 0x454F3FAE, "WATER_REF_malibu" },
            { 0xA3810B89, "carpark_dt1_02" },
            { 0x5D719FD7, "FIB_interview_optimise" },
            { 0x26A17762, "Prologue_shootout_opt" },
            { 0xBAEF53F9, "hangar_lightsmod" },
            { 0x2C70810F, "plane_inside_mode" },
            { 0x4E213E70, "eatra_bouncelight_beach" },
            { 0x6EC8FB39, "downtown_FIB_cascades_opt" },
            { 0x4E25A22C, "jewel_optim" },
            { 0xE3D0F78B, "gorge_reflectionoffset2" },
            { 0x3911A0DD, "ufo_deathray" },
            { 0xB1DC7422, "PORT_heist_underwater" },
            { 0xBACB97F0, "TUNNEL_orange_exterior" },
            { 0x7A55187E, "hillstunnel" },
            { 0x4F32361D, "jewelry_entrance_INT" },
            { 0x6D074BD3, "jewelry_entrance" },
            { 0x2CCB0F73, "jewelry_entrance_INT_fog" },
            { 0xBC7704CF, "TUNNEL_yellow_ext" },
            { 0x313B7ED4, "NEW_jewel_EXIT" },
            { 0x1C4DB7CD, "services_nightlight" },
            { 0x8331E0AD, "CS1_railwayB_tunnel" },
            { 0xE4178367, "TUNNEL_green_ext" },
            { 0x5F4CF7DD, "CAMERA_secuirity" },
            { 0x93787E89, "CAMERA_secuirity_FUZZ" },
            { 0x34E98C98, "int_hospital_small" },
            { 0xD6EF3154, "int_hospital_dark" },
            { 0xACA6B60A, "plaza_carpark" },
            { 0x55C419A1, "gen_bank" },
            { 0xB3D1961F, "nightvision" },
            { 0x17B97383, "WATER_cove" },
            { 0xD171EA65, "glasses_Darkblue" },
            { 0x873A3ED7, "glasses_VISOR" },
            { 0x3D978F1D, "heist_boat" },
            { 0x9B649952, "heist_boat_norain" },
            { 0x6810EA94, "heist_boat_engineRoom" },
            { 0x57DDF92C, "buggy_shack" },
            { 0x891C81E7, "mineshaft" },
            { 0x3CBF7EBF, "NG_first" },
            { 0x900FE9E2, "glasses_Scuba" },
            { 0x5C0E7959, "mugShot" },
            { 0xB9D16FDB, "Glasses_BlackOut" },
            { 0x01305F68, "winning_room" },
            { 0xD16C05E5, "mugShot_lineup" },
            { 0x0D81A17A, "MPApartHigh_palnning" },
            { 0x50E7A950, "v_dark" },
            { 0xA675F3BF, "vehicle_subint" },
            { 0xEFF44B69, "Carpark_MP_exit" },
            { 0x061217C5, "EXT_FULLAmbientmult_art" },
            { 0x63DB1CFD, "new_MP_Garage_L" },
            { 0x3920C61E, "fp_vig_black" },
            { 0x218E2927, "fp_vig_brown" },
            { 0x3549CB08, "fp_vig_gray" },
            { 0xA5EF5102, "fp_vig_blue" },
            { 0x690F470E, "fp_vig_red" },
            { 0xD576542C, "fp_vig_green" },
            { 0x1A7060DA, "INT_trailer_cinema" },
            { 0x7D675DD9, "heliGunCam" },
            { 0x7345CF82, "INT_smshop" },
            { 0xDC963531, "INT_mall" },
            { 0x1229ECE1, "Mp_Stilts" },
            { 0x4F65272B, "Mp_Stilts_gym" },
            { 0x0C35703B, "Mp_Stilts2" },
            { 0x3ECA052D, "Mp_Stilts_gym2" },
            { 0xB0CAA463, "MPApart_H_01" },
            { 0x57E08D78, "MPApart_H_01_gym" },
            { 0xE14C49DE, "MP_H_01_Study" },
            { 0xE576E36C, "MP_H_01_Bedroom" },
            { 0xBFF65A20, "MP_H_01_Bathroom" },
            { 0x1A1CC69B, "MP_H_01_New" },
            { 0xDCBB49FC, "MP_H_01_New_Bedroom" },
            { 0xE99D9B46, "MP_H_01_New_Bathroom" },
            { 0xB2EAF704, "MP_H_01_New_Study" },
            { 0x3BA0C811, "INT_smshop_inMOD" },
            { 0x325F63D3, "NoPedLight" },
            { 0x08F2C678, "morgue_dark_ovr" },
            { 0x55361615, "INT_smshop_outdoor_bloom" },
            { 0x0EF26B17, "INT_smshop_indoor_bloom" },
            { 0x644EF503, "MP_H_02" },
            { 0x455C371E, "MP_H_04" },
            { 0xB6054341, "Mp_Stilts2_bath" },
            { 0x12F75269, "mp_h_05" },
            { 0xFAEB2251, "mp_h_07" },
            { 0xED2F86DA, "MP_H_06" },
            { 0xD1ACCFD5, "mp_h_08" },
            { 0x6F4FC336, "yacht_DLC" },
            { 0x479B125F, "mp_exec_office_01" },
            { 0x21655D98, "mp_exec_warehouse_01" },
            { 0x796DF604, "mp_exec_office_02" },
            { 0x332A697E, "mp_exec_office_03" },
            { 0x254BCDC1, "mp_exec_office_04" },
            { 0x57CB32B7, "mp_exec_office_05" },
            { 0x49A5966C, "mp_exec_office_06" },
            { 0x308EB7DB, "mp_exec_office_03_blue" },
            { 0x40765EF0, "mp_exec_office_03C" },
            { 0x7252FC8F, "mp_bkr_int01_garage" },
            { 0xEEF9DF99, "mp_bkr_int01_transition" },
            { 0x99D19199, "mp_bkr_int01_small_rooms" },
            { 0x5C07B131, "mp_bkr_int02_garage" },
            { 0x0C7F45E2, "mp_bkr_int02_hangout" },
            { 0xCE490066, "mp_bkr_int02_small_rooms" },
            { 0x2907A081, "mp_bkr_ware01" },
            { 0xAE2706BB, "mp_bkr_ware02_standard" },
            { 0x117E4717, "mp_bkr_ware02_upgrade" },
            { 0xDB34A96D, "mp_bkr_ware02_dry" },
            { 0x6F86F880, "mp_bkr_ware03_basic" },
            { 0x69DB963C, "mp_bkr_ware03_upgrade" },
            { 0x3F6ACD47, "mp_bkr_ware04" },
            { 0x85A4D9BA, "mp_bkr_ware05" },
            { 0x18AD3454, "mp_lad_night" },
            { 0x7BDCF699, "mp_lad_day" },
            { 0xF4D88ED8, "mp_lad_judgment" },
            { 0x35D210E4, "mp_imx_intwaremed" },
            { 0x2DC3F8D1, "mp_imx_intwaremed_office" },
            { 0xA458F4B4, "mp_imx_mod_int_01" },
            { 0xE2832FAF, "IMpExt_Interior_02" },
            { 0xD0E94091, "ImpExp_Interior_01" },
            { 0x0EA0291B, "impexp_interior_01_lift" },
            { 0x8C8D2B0B, "IMpExt_Interior_02_stair_cage" },
            { 0x8324DE82, "mp_gr_int01_white" },
            { 0x861776C1, "mp_gr_int01_grey" },
            { 0xEEB62D1E, "mp_gr_int01_black" },
            { 0xF29672E3, "grdlc_int_02" },
            { 0x87A7BDB8, "mp_nightshark_shield_fp" },
            { 0xD8998C7E, "grdlc_int_02_trailer_cave" },
            { 0xD549D54C, "polluted" },
            { 0xC741AEAD, "lightning" },
            { 0x218ACD6E, "torpedo" },
            { 0x8EC030E9, "NEW_shrinksOffice" },
            { 0x842C3A35, "Facebook_NEW" },
            { 0xAA2EB653, "NEW_trevorstrailer" },
            { 0x8696694C, "New_sewers" },
            { 0x5EFB4B5A, "facebook_serveroom" },
            { 0xEE66BBBC, "V_Office_smoke_Fire" },
            { 0xEFE5A60F, "V_FIB_IT3_alt5" },
            { 0xFA78D98C, "int_Hospital_DM" },
            { 0x9EDFA39B, "int_Hospital2_DM" },
            { 0xBEAB7CB9, "int_Barber1" },
            { 0xEF1AD626, "int_tattoo_B" },
            { 0x09DC1578, "glasses_black" },
            { 0xE9F0FCB0, "glasses_brown" },
            { 0x3C09CF60, "glasses_blue" },
            { 0x5DC6C2B8, "glasses_red" },
            { 0x9245127B, "glasses_green" },
            { 0x8A0C3897, "glasses_yellow" },
            { 0xFFB7CB29, "glasses_purple" },
            { 0x21643B34, "glasses_pink" },
            { 0x5282E4C4, "glasses_orange" },
            { 0xA0AEE342, "WATER_ID2_21" },
            { 0x09522B76, "WATER_RichmanStuntJump" },
            { 0x7376E13D, "CH3_06_water" },
            { 0x76FCBDB6, "WATER_refmap_hollywoodlake" },
            { 0x27D2FEA0, "WATER_CH2_06_01_03" },
            { 0x2D8FBFB4, "WATER_CH2_06_02" },
            { 0x99D39842, "WATER_CH2_06_04" },
            { 0xDF2FA472, "RemoteSniper" },
            { 0xF0900DF6, "V_Office_smoke" },
            { 0x849C9A23, "V_Office_smoke_ext" },
            { 0x43FA2EA2, "V_FIB_IT3" },
            { 0x16DAD556, "V_FIB_IT3_alt" },
            { 0x775AC8E3, "V_FIB_stairs" },
            { 0x91C11002, "v_abattoir" },
            { 0xB430CE7A, "V_Abattoir_Cold" },
            { 0x7329ADEF, "v_recycle" },
            { 0x4E55A04E, "v_strip3" },
            { 0x3F2BA3B3, "v_strpchangerm" },
            { 0x01BF0D00, "v_jewel2" },
            { 0x05314B05, "v_foundry" },
            { 0x93D1B362, "V_Metro_station" },
            { 0xF330BFC0, "v_metro" },
            { 0x94B1B3CC, "V_Metro2" },
            { 0x2AF87AE0, "v_torture" },
            { 0x5DE36999, "v_sweat" },
            { 0x28C57E42, "v_sweat_entrance" },
            { 0x92F6193F, "v_sweat_NoDirLight" },
            { 0x95A58E67, "Barry1_Stoned" },
            { 0x56DC69DB, "v_rockclub" },
            { 0xC31E7C77, "v_michael" },
            { 0xC2129ACD, "v_michael_lounge" },
            { 0xFCDBE127, "v_janitor" },
            { 0x79667A2A, "int_amb_mult_large" },
            { 0x14F1D0EF, "int_extlight_large" },
            { 0x76149E43, "ext_int_extlight_large" },
            { 0xCDE50982, "int_extlight_small" },
            { 0x3DA44B3A, "int_extlight_small_clipped" },
            { 0xE9BDA156, "int_extlight_large_fog" },
            { 0x4A067B38, "int_extlight_small_fog" },
            { 0x120D2A19, "int_extlight_none" },
            { 0x31A4ABCD, "int_extlight_none_dark" },
            { 0x2DA9D2A7, "int_extlight_none_dark_fog" },
            { 0x98BBDE07, "int_extlight_none_fog" },
            { 0xC1197815, "int_clean_extlight_large" },
            { 0xA321B1C1, "int_clean_extlight_small" },
            { 0x24A1053A, "int_clean_extlight_none" },
            { 0x41B63FE9, "prologue" },
            { 0xAA022B5F, "vagos_extlight_small" },
            { 0x3EBBDACB, "FinaleBank" },
            { 0xE003E3B6, "FinaleBankMid" },
            { 0x3E507593, "v_cashdepot" },
            { 0xB5E0C0B3, "V_Solomons" },
            { 0x99057576, "int_methlab_small" },
            { 0x2452F713, "int_Lost_small" },
            { 0xC9F95FE4, "int_Lost_none" },
            { 0x9DB79874, "int_ControlTower_small" },
            { 0x6F41369F, "int_ControlTower_none" },
            { 0x934D2D80, "int_dockcontrol_small" },
            { 0x8216F464, "int_hanger_small" },
            { 0xFB2B59CF, "int_hanger_none" },
            { 0xFC850175, "int_cluckinfactory_small" },
            { 0xB6BF8E88, "int_cluckinfactory_none" },
            { 0xE83C53FC, "int_FranklinAunt_small" },
            { 0xFF10FDEE, "stc_franklinsHouse" },
            { 0x2EAAC584, "stc_coroners" },
            { 0x96CEB1B8, "stc_trevors" },
            { 0x8635CE33, "stc_deviant_lounge" },
            { 0x3992E50E, "stc_deviant_bedroom" },
            { 0x3CBA299F, "int_carshowroom" },
            { 0x0ADAE47C, "int_Farmhouse_small" },
            { 0x68E993DB, "int_Farmhouse_none" },
            { 0x1898B55F, "int_carmod_small" },
            { 0xB13253D4, "SP1_03_drawDistance" },
            { 0x924D8823, "int_clotheslow_large" },
            { 0xDE39E103, "v_bahama" },
            { 0xD53E3D2E, "gunclub" },
            { 0x42225664, "int_GasStation" },
            { 0x99A4BB09, "PoliceStation" },
            { 0xBF262E14, "PoliceStationDark" },
            { 0x7197E390, "Shop247" },
            { 0x5D4CF6B4, "Shop247_none" },
            { 0xEA391B9D, "Hicksbar" },
            { 0x550E3B12, "cBank_back" },
            { 0xF261A9BA, "cBank_front" },
            { 0x99A5B82A, "int_office_Lobby" },
            { 0x44B58203, "int_office_LobbyHall" },
            { 0x07A355AA, "SheriffStation" },
            { 0x1DA56CF1, "LifeInvaderLOD" },
            { 0x0B791B05, "int_motelroom" },
            { 0xB5FDB40E, "metro" },
            { 0x7C6C2373, "int_ClothesHi" },
            { 0x6EEC2031, "FIB_5" },
            { 0x466DE80E, "int_chopshop" },
            { 0x77A1AD09, "int_tattoo" },
            { 0xAAD7F0A4, "gunstore" },
            { 0xB81C424E, "int_Hospital_Blue" },
            { 0x3CAFBBB9, "FIB_6" },
            { 0x5F3980F4, "FIB_B" },
            { 0x92816783, "FIB_A" },
            { 0x090FBB5A, "lab_none" },
            { 0xB0FB4830, "lab_none_dark" },
            { 0x237EDA70, "lab_none_dark_fog" },
            { 0x47C88E0F, "MP_Garage_L" },
            { 0x14B8D4FD, "MP_Studio_Lo" },
            { 0xB4AF5AAB, "StadLobby" },
            { 0x638EDF7D, "Hanger_INTmods" },
            { 0x93D424CB, "MPApartHigh" },
            { 0x0E2F23F2, "int_Hospital_BlueB" },
            { 0xB8FC3634, "int_tunnel_none_dark" },
            { 0x1C0919F9, "MP_lowgarage" },
            { 0xD60C2806, "MP_MedGarage" },
            { 0xD619777A, "shades_yellow" },
            { 0x5C07336E, "shades_pink" },
            { 0x1514C4C7, "Mp_apart_mid" },
            { 0x450B0FC0, "yell_tunnel_nodirect" },
            { 0xA14B51BD, "int_carrier_hanger" },
            { 0xEACE3F51, "int_carrier_stair" },
            { 0x507BDD2F, "int_carrier_rear" },
            { 0x74463779, "int_carrier_control" },
            { 0xEF39CF61, "int_carrier_control_2" },
            { 0xE4DF46D5, "default" },
            { 0x2EDF4AFC, "gunshop" },
            { 0x8380EF81, "MichaelsDirectional" },
            { 0x4A9C4CEE, "Bank_HLWD" },
            { 0xAF2F61FB, "MichaelsNODirectional" },
            { 0xB50EDE40, "MichaelsDarkroom" },
            { 0x7D279D6A, "int_lesters" },
            { 0x2CC1782C, "Tunnel_green1" },
            { 0x2DEB3533, "cinema_001" },
            { 0xBCDBCD76, "exile1_plane" },
            { 0x0C09DDC9, "player_transition" },
            { 0x91489BBE, "player_transition_no_scanlines" },
            { 0x46CA16BD, "player_transition_scanlines" },
            { 0x9C7FAC59, "switch_cam_1" },
            { 0x67D442FF, "switch_cam_2" },
            { 0x4D0DDED7, "Bloom" },
            { 0x787C5B5B, "BloomLight" },
            { 0x6DC6DA94, "BloomMid" },
            { 0x42F8390B, "DrivingFocusLight" },
            { 0xC096B564, "DrivingFocusDark" },
            { 0x6596E830, "RaceTurboLight" },
            { 0xB5937A6E, "RaceTurboDark" },
            { 0x616E35F3, "BulletTimeLight" },
            { 0xB799C9F2, "BulletTimeDark" },
            { 0xE4357941, "REDMIST" },
            { 0x47DF734E, "REDMIST_blend" },
            { 0xA51FD197, "MP_Bull_tost" },
            { 0xAE89B5A7, "MP_Bull_tost_blend" },
            { 0xAB722F6A, "MP_Powerplay" },
            { 0xB5A9B23B, "MP_Powerplay_blend" },
            { 0xE0940793, "MP_Killstreak" },
            { 0xA7C4AEBB, "MP_Killstreak_blend" },
            { 0x36AE88CD, "MP_Loser" },
            { 0x8807C48C, "MP_Loser_blend" },
            { 0xFA543B0A, "CHOP" },
            { 0xC58F1D01, "FranklinColorCode" },
            { 0x2E55FF72, "MichaelColorCode" },
            { 0x9B162E21, "TrevorColorCode" },
            { 0x369C486D, "NeutralColorCode" },
            { 0x0B674FA4, "NeutralColorCodeLight" },
            { 0x87D38C02, "FranklinColorCodeBasic" },
            { 0x87A9FA29, "MichaelColorCodeBasic" },
            { 0xAFE3459C, "TrevorColorCodeBasic" },
            { 0x42353404, "NeutralColorCodeBasic" },
            { 0xD01920E8, "DefaultColorCode" },
            { 0xA6F09067, "PlayerSwitchPulse" },
            { 0x63EC45A2, "PlayerSwitchNeutralFlash" },
            { 0x7EB18C06, "hud_def_lensdistortion" },
            { 0x22F99A81, "hud_def_lensdistortion_subtle" },
            { 0x5047ABBA, "hud_def_blur" },
            { 0xCFB1EB3D, "hud_def_colorgrade" },
            { 0x6E3CF0A6, "hud_def_flash" },
            { 0xFD5DA255, "hud_def_desatcrunch" },
            { 0xCAD8A662, "hud_def_desat_switch" },
            { 0xBF21F64B, "hud_def_desat_cold" },
            { 0x179561B6, "hud_def_desat_cold_kill" },
            { 0x52F1BC81, "hud_def_desat_Neutral" },
            { 0x60434FEA, "hud_def_focus" },
            { 0x813C811F, "hud_def_desat_Franklin" },
            { 0x048FC911, "hud_def_desat_Michael" },
            { 0xF37BCC65, "hud_def_desat_Trevor" },
            { 0x5F13AC77, "hud_def_Franklin" },
            { 0xCA9AAC7D, "hud_def_Michael" },
            { 0x21FA71EC, "hud_def_Trevor" },
            { 0xBEB8FD61, "michealspliff" },
            { 0xE51C79DE, "michealspliff_blend" },
            { 0x268C003F, "michealspliff_blend02" },
            { 0x138D7C18, "trevorspliff" },
            { 0x4F1E38CE, "trevorspliff_blend" },
            { 0x30791289, "trevorspliff_blend02" },
            { 0xC3A46991, "BarryFadeOut" },
            { 0x1C70C68D, "stoned" },
            { 0xE0408739, "stoned_cutscene" },
            { 0xA5A70F6D, "stoned_monkeys" },
            { 0xA426064E, "stoned_aliens" },
            { 0x3FDBA102, "Drunk" },
            { 0x17B894F0, "drug_flying_base" },
            { 0xB7E9CFB4, "drug_flying_01" },
            { 0xA5D72B8F, "drug_flying_02" },
            { 0x874586CD, "DRUG_gas_huffin" },
            { 0x0BD6B639, "Drug_deadman" },
            { 0x348207D9, "Drug_deadman_blend" },
            { 0x816E33E8, "DRUG_2_drive" },
            { 0x1CA108B7, "drug_drive_blend01" },
            { 0x01E7D345, "drug_drive_blend02" },
            { 0x2356FC01, "drug_wobbly" },
            { 0xB766CC15, "Dont_tazeme_bro" },
            { 0xC8F9F411, "dont_tazeme_bro_b" },
            { 0x660D4F7C, "int_extlght_sm_cntrst" },
            { 0x642AE872, "MP_heli_cam" },
            { 0x8569D239, "helicamfirst" },
            { 0xA5BEA528, "introblue" },
            { 0x0808341F, "MP_select" },
            { 0xEAA42EE3, "PERSHING_water_reflect" },
            { 0x5232C0FE, "exile1_exit" },
            { 0xA2663B14, "phone_cam" },
            { 0x4BE17F8E, "ExplosionJosh" },
            { 0x89A8CBF1, "RaceTurboFlash" },
            { 0x08F8BB4A, "MP_death_grade" },
            { 0x28E5A030, "MP_death_grade_blend01" },
            { 0x1F948D8E, "MP_death_grade_blend02" },
            { 0x17615E1C, "NG_deathfail_BW_base" },
            { 0x79E72168, "NG_deathfail_BW_blend01" },
            { 0x6F750C84, "NG_deathfail_BW_blend02" },
            { 0x8ED2A38A, "MP_job_win" },
            { 0x681F9FFD, "MP_job_lose" },
            { 0x787E7EE6, "MP_corona_tournament" },
            { 0xDEB8E3DF, "MP_corona_heist" },
            { 0x11C6C4DD, "MP_corona_selection" },
            { 0x2B402288, "WhiteOut" },
            { 0x465B313A, "BlackOut" },
            { 0x9754FCB6, "MP_job_load" },
            { 0x9C649D97, "MP_intro_logo" },
            { 0x19A820C2, "MP_corona_switch" },
            { 0xAD0F54FB, "MP_race_finish" },
            { 0xFAC549DD, "phone_cam1" },
            { 0x0FE77421, "phone_cam2" },
            { 0x963100B6, "phone_cam3" },
            { 0xAB6C2B2C, "phone_cam4" },
            { 0x294FA745, "phone_cam5" },
            { 0x17AA03FA, "phone_cam6" },
            { 0x8DE4706D, "phone_cam7" },
            { 0x7063356B, "phone_cam9" },
            { 0xA5E85059, "phone_cam10" },
            { 0x5DA8BFDB, "phone_cam11" },
            { 0x4A561936, "phone_cam12" },
            { 0x6FFC6482, "phone_cam13" },
            { 0x40958120, "FranklinColorCodeBright" },
            { 0xF0B28E76, "MichaelColorCodeBright" },
            { 0xB741914E, "TrevorColorCodeBright" },
            { 0xDA824B38, "NeutralColorCodeBright" },
            { 0x86C1C123, "Kifflom" },
            { 0x48CE2C36, "MP_job_load_01" },
            { 0xFC559346, "MP_job_load_02" },
            { 0xEDEA1528, "MP_job_preload" },
            { 0x593270D1, "MP_job_preload_blend" },
            { 0xBF90F556, "NG_filmnoir_BW01" },
            { 0xB1D3D9DC, "NG_filmnoir_BW02" },
            { 0xFEC1314E, "NG_filmic01" },
            { 0x44C8BD68, "NG_filmic02" },
            { 0x330719E5, "NG_filmic03" },
            { 0xE72A822D, "NG_filmic04" },
            { 0x156BDEAF, "NG_filmic05" },
            { 0x7B9A2AFE, "NG_filmic06" },
            { 0x69540672, "NG_filmic07" },
            { 0x5F0071D7, "NG_filmic08" },
            { 0x4C414C59, "NG_filmic09" },
            { 0xFB042B40, "NG_filmic10" },
            { 0x4D0A4F4B, "NG_filmic11" },
            { 0x16DFE2F7, "NG_filmic12" },
            { 0xE8840640, "NG_filmic13" },
            { 0x93AC5C82, "NG_filmic14" },
            { 0x96F8631A, "NG_filmic15" },
            { 0x2FAB148D, "NG_filmic16" },
            { 0x425E39F3, "NG_filmic17" },
            { 0x6CFB0F2C, "NG_filmic18" },
            { 0xCE65D200, "NG_filmic19" },
            { 0xDE51E9D4, "NG_filmic20" },
            { 0x74F36FA1, "AP1_01_C_NoFog" },
            { 0x32E96322, "AP1_01_B_IntRefRange" },
            { 0x85BF028A, "rply_saturation" },
            { 0x53E8448C, "rply_saturation_neg" },
            { 0x4F6992F5, "rply_vignette" },
            { 0x8813365E, "rply_vignette_neg" },
            { 0xAFFD7204, "rply_contrast" },
            { 0xDDD00563, "rply_contrast_neg" },
            { 0x4685F55F, "rply_brightness" },
            { 0x33EDE908, "rply_brightness_neg" },
            { 0x2D0208A7, "rply_motionblur" },
            { 0xAC2D2688, "V_CIA_Facility" },
            { 0xDF807B19, "hud_def_blur_switch" },
            { 0x295BE3D2, "MP_corona_tournament_DOF" },
            { 0x07F91B01, "MP_corona_heist_blend" },
            { 0xA745F7C1, "MP_corona_heist_DOF" },
            { 0x4AAC069C, "MP_corona_heist_BW" },
            { 0xBD5BFEA3, "phone_cam3_REMOVED" },
            { 0x047EA8C0, "phone_cam8_REMOVED" },
            { 0x846D5D7F, "phone_cam8" },
            { 0x6202A1B9, "lab_none_exit_OVR" },
            { 0x5730AE15, "lab_none_dark_OVR" },
            { 0x8B0C032E, "LectroLight" },
            { 0x1F837C7F, "LectroDark" },
            { 0xAFA00C71, "NG_filmic21" },
            { 0x49A64083, "NG_filmic22" },
            { 0x5B1BE36E, "NG_filmic23" },
            { 0x74FA9727, "NG_filmic24" },
            { 0x8744BBBB, "NG_filmic25" },
            { 0x8366C52C, "NG_blackout" },
            { 0xEFD37CEA, "MP_deathfail_night" },
            { 0xD16586AB, "lodscaler" },
            { 0x65C477DF, "maxlodscaler" },
            { 0xFEE7F182, "MP_job_preload_night" },
            { 0x4F7EC733, "MP_job_end_night" },
            { 0xB1E083DC, "MP_corona_heist_BW_night" },
            { 0x02D63988, "MP_corona_heist_night" },
            { 0x8CD6E6AE, "MP_corona_heist_night_blend" },
            { 0xA29682F2, "PennedInLight" },
            { 0xAF1F8EF1, "PennedInDark" },
            { 0x47027C42, "BeastLaunch01" },
            { 0x1AAC2396, "BeastLaunch02" },
            { 0xAB1BBCF4, "BeastIntro01" },
            { 0xFCEBE027, "BeastIntro02" },
            { 0x00C98A12, "CrossLine01" },
            { 0x177EB77C, "CrossLine02" },
            { 0x2DFF8D5D, "InchOrange01" },
            { 0x70BC12D5, "InchOrange02" },
            { 0x8FCF087B, "InchPurple01" },
            { 0xCA97FE0C, "InchPurple02" },
            { 0xBF1EA6A9, "TinyPink01" },
            { 0xCD78435C, "TinyPink02" },
            { 0x71F45C5E, "TinyGreen01" },
            { 0x5F593728, "TinyGreen02" },
            { 0x5CC7014F, "InchPickup01" },
            { 0x6EE1A584, "InchPickup02" },
            { 0xDB4E7DF5, "PPOrange01" },
            { 0xC890D87A, "PPOrange02" },
            { 0x2B9B2080, "PPPurple01" },
            { 0x42E54F14, "PPPurple02" },
            { 0x5CAECDD2, "PPGreen01" },
            { 0x6A75695F, "PPGreen02" },
            { 0xBA0D468C, "PPPink01" },
            { 0x0C67EB40, "PPPink02" },
            { 0xB3146391, "StuntSlowLight" },
            { 0x6341D5E9, "StuntSlowDark" },
            { 0x669F2C17, "StuntFastLight" },
            { 0x1176D764, "StuntFastDark" },
            { 0x51C568C1, "PPFilter" },
            { 0x38ABA79A, "BikerFilter" },
            { 0x79C1283C, "LostTimeLight" },
            { 0x128CF427, "LostTimeDark" },
            { 0xC07E5C3F, "LostTimeFlash" },
            { 0xDBF0DDD1, "DeadlineNeon01" },
            { 0x5AE150A1, "BikerFormFlash" },
            { 0xD600A81B, "BikerForm01" },
            { 0xF32DF3E9, "VolticBlur" },
            { 0x02EAABAE, "VolticFlash" },
            { 0x89992DE1, "VolticGold" },
            { 0x87176034, "BleepYellow01" },
            { 0x75D83DB6, "BleepYellow02" },
            { 0x6110928D, "TinyRacerMoBlur" },
            { 0x0001B512, "WeaponUpgrade" },
            { 0xC8C008A6, "AirRaceBoost01" },
            { 0x12041B2D, "AirRaceBoost02" },
            { 0x93953450, "TransformRaceFlash" },
            { 0x889B892E, "BombCamFlash" },
            { 0xCA702997, "BombCam01" },
            { 0x0733A4F3, "WarpCheckpoint" },
        };
    }

    /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierModsCollection/Doc/*' />
    public sealed unsafe class TimeCycleModifierModsCollection : IEnumerable<TimeCycleModifierMod>
    {
        internal readonly TimeCycleModifier Modifier;
        private readonly Dictionary<TimeCycleModifierModType, TimeCycleModifierMod> modByType;

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierModsCollection/Member[@name="Count"]/*' />
        public int Count
        {
            get
            {
                CTimeCycleModifier* native = (CTimeCycleModifier*)Modifier.MemoryAddress;
                return native->Mods.Count;
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierModsCollection/Member[@name="Indexer1"]/*' />
        public TimeCycleModifierMod this[int index]
        {
            get
            {
                CTimeCycleModifier* native = (CTimeCycleModifier*)Modifier.MemoryAddress;
                if (index < 0 || index >= native->Mods.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                CTimeCycleModifier.Mod* nativeMod = native->Mods.Get((short)index);
                TimeCycleModifierModType type = (TimeCycleModifierModType)nativeMod->ModType;

                if (modByType.TryGetValue(type, out TimeCycleModifierMod m))
                {
                    return m;
                }
                else
                {
                    TimeCycleModifierMod managedMod = new TimeCycleModifierMod(this, type);
                    modByType[type] = managedMod;
                    return managedMod;
                }
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierModsCollection/Member[@name="Indexer2"]/*' />
        public TimeCycleModifierMod this[TimeCycleModifierModType type]
        {
            get
            {
                if (modByType.TryGetValue(type, out TimeCycleModifierMod m))
                {
                    return m.IsValid ? m : throw new InvalidOperationException($"This {nameof(TimeCycleModifierModsCollection)} doesn't contain a mod of type {type}.");
                }
                else
                {
                    if(GetIndexForType(type) == -1)
                        throw new InvalidOperationException($"This {nameof(TimeCycleModifierModsCollection)} doesn't contain a mod of type {type}.");
                    TimeCycleModifierMod managedMod = new TimeCycleModifierMod(this, type);
                    modByType[type] = managedMod;
                    return managedMod;
                }
            }
        }

        internal TimeCycleModifierModsCollection(TimeCycleModifier modifier)
        {
            Modifier = modifier;
            modByType = new Dictionary<TimeCycleModifierModType, TimeCycleModifierMod>();
        }

        internal short GetIndexForType(TimeCycleModifierModType type)
        {
            CTimeCycleModifier* native = (CTimeCycleModifier*)Modifier.MemoryAddress;
            for (short i = 0; i < native->Mods.Count; i++)
            {
                CTimeCycleModifier.Mod* mod = native->Mods.Get(i);
                if (mod->ModType == (int)type)
                {
                    return i;
                }
                else if (i >= (int)type) // the mods array is sorted by type value, if current index is equal to or greater than type value,
                                         // we can safely assume the array doesn't contain a mod of that type and break out of it
                {
                    break;
                }
            }

            return -1;
        }


        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierModsCollection/Member[@name="Has"]/*' />
        public bool Has(TimeCycleModifierModType type)
        {
            return GetIndexForType(type) != -1;
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierModsCollection/Member[@name="Add"]/*' />
        public TimeCycleModifierMod Add(TimeCycleModifierModType type, float value1, float value2)
        {
            if(Has(type))
                throw new InvalidOperationException($"This {nameof(TimeCycleModifierModsCollection)} already contains a mod of type {type}.");

            CTimeCycleModifier* native = (CTimeCycleModifier*)Modifier.MemoryAddress;

            CTimeCycleModifier.Mod* newMod = native->GetUnusedModEntry();
            newMod->ModType = (int)type;
            newMod->Value1 = value1;
            newMod->Value2 = value2;

            native->SortMods();

            TimeCycleModifierMod managedMod = new TimeCycleModifierMod(this, type);
            modByType[type] = managedMod;
            return managedMod;
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierModsCollection/Member[@name="Remove"]/*' />
        public void Remove(TimeCycleModifierModType type)
        {
            short index = GetIndexForType(type);

            if (index == -1)
                throw new InvalidOperationException($"This {nameof(TimeCycleModifierModsCollection)} doesn't contain a mod of type {type}.");

            CTimeCycleModifier* native = (CTimeCycleModifier*)Modifier.MemoryAddress;
            native->RemoveModEntry(index);

            if (modByType.ContainsKey(type))
                modByType.Remove(type);
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierModsCollection/Member[@name="GetEnumerator"]/*' />
        public IEnumerator<TimeCycleModifierMod> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierModsCollection/Member[@name="GetEnumerator"]/*' />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierMod/Doc/*' />
    public sealed unsafe class TimeCycleModifierMod : IAddressable, IEquatable<TimeCycleModifierMod>
    {
        private readonly TimeCycleModifierModsCollection collection;
        private readonly TimeCycleModifierModType type;

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierMod/Member[@name="Index"]/*' />
        public int Index { get { return collection.GetIndexForType(type); } }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierMod/Member[@name="MemoryAddress"]/*' />
        public IntPtr MemoryAddress
        {
            get
            {
                return (IntPtr)GetNative();
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierMod/Member[@name="Type"]/*' />
        public TimeCycleModifierModType Type { get { return type; } }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierMod/Member[@name="Value1"]/*' />
        public float Value1
        {
            get { return IsValid ? GetNative()->Value1 : throw new InvalidOperationException(); }
            set
            {
                if (!IsValid)
                    throw new InvalidOperationException();
                GetNative()->Value1 = value;
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierMod/Member[@name="Value2"]/*' />
        public float Value2
        {
            get { return IsValid ? GetNative()->Value2 : throw new InvalidOperationException(); }
            set
            {
                if (!IsValid)
                    throw new InvalidOperationException();
                GetNative()->Value2 = value;
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierMod/Member[@name="IsValid"]/*' />
        public bool IsValid
        {
            get { return Index != -1; }
        }

        internal TimeCycleModifierMod(TimeCycleModifierModsCollection collection, TimeCycleModifierModType type)
        {
            this.collection = collection;
            this.type = type;
        }

        private CTimeCycleModifier.Mod* GetNative()
        {
            short index = collection.GetIndexForType(type);
            if (index < 0)
                return null;

            CTimeCycleModifier* native = (CTimeCycleModifier*)collection.Modifier.MemoryAddress;
            if (index >= native->Mods.Count)
                throw new IndexOutOfRangeException();

            return native->Mods.Get(index);
        }

        public override bool Equals(object obj)
        {
            if(obj is TimeCycleModifierMod other)
            {
                return Equals(other);
            }

            return false;
        }

        public bool Equals(TimeCycleModifierMod other)
        {
            return other != null && other.type == type && ReferenceEquals(other.collection, collection);
        }

        public override int GetHashCode()
        {
            return unchecked(collection.GetHashCode() * 17 + type.GetHashCode());
        }


        public static bool operator ==(TimeCycleModifierMod left, TimeCycleModifierMod right)
        {
            if (left == null)
                return right == null;
            return left.Equals(right);
        }

        public static bool operator !=(TimeCycleModifierMod left, TimeCycleModifierMod right)
        {
            return !(left == right);
        }
    }

    /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierModType/Doc/*' />
    public enum TimeCycleModifierModType
    {
        light_dir_col_r = 0,
        light_dir_col_g = 1,
        light_dir_col_b = 2,
        light_dir_mult = 3,
        light_directional_amb_col_r = 4,
        light_directional_amb_col_g = 5,
        light_directional_amb_col_b = 6,
        light_directional_amb_intensity = 7,
        light_directional_amb_intensity_mult = 8,
        light_directional_amb_bounce_enabled = 9,
        light_amb_down_wrap = 10,
        light_natural_amb_down_col_r = 11,
        light_natural_amb_down_col_g = 12,
        light_natural_amb_down_col_b = 13,
        light_natural_amb_down_intensity = 14,
        light_natural_amb_up_col_r = 15,
        light_natural_amb_up_col_g = 16,
        light_natural_amb_up_col_b = 17,
        light_natural_amb_up_intensity = 18,
        light_natural_amb_up_intensity_mult = 19,
        light_natural_push = 20,
        light_ambient_bake_ramp = 21,
        light_artificial_int_down_col_r = 22,
        light_artificial_int_down_col_g = 23,
        light_artificial_int_down_col_b = 24,
        light_artificial_int_down_intensity = 25,
        light_artificial_int_up_col_r = 26,
        light_artificial_int_up_col_g = 27,
        light_artificial_int_up_col_b = 28,
        light_artificial_int_up_intensity = 29,
        light_artificial_ext_down_col_r = 30,
        light_artificial_ext_down_col_g = 31,
        light_artificial_ext_down_col_b = 32,
        light_artificial_ext_down_intensity = 33,
        light_artificial_ext_up_col_r = 34,
        light_artificial_ext_up_col_g = 35,
        light_artificial_ext_up_col_b = 36,
        light_artificial_ext_up_intensity = 37,
        ped_light_col_r = 38,
        ped_light_col_g = 39,
        ped_light_col_b = 40,
        ped_light_mult = 41,
        ped_light_direction_x = 42,
        ped_light_direction_y = 43,
        ped_light_direction_z = 44,
        light_amb_occ_mult = 45,
        light_amb_occ_mult_ped = 46,
        light_amb_occ_mult_veh = 47,
        light_amb_occ_mult_prop = 48,
        light_amb_volumes_in_diffuse = 49,
        ssao_inten = 50,
        ssao_type = 51,
        ssao_cp_strength = 52,
        ssao_qs_strength = 53,
        light_ped_rim_mult = 54,
        light_dynamic_bake_tweak = 55,
        light_vehicle_second_spec_override = 56,
        light_vehicle_intenity_scale = 57,
        light_direction_override = 58,
        light_direction_override_overrides_sun = 59,
        sun_direction_x = 60,
        sun_direction_y = 61,
        sun_direction_z = 62,
        moon_direction_x = 63,
        moon_direction_y = 64,
        moon_direction_z = 65,
        light_ray_col_r = 66,
        light_ray_col_g = 67,
        light_ray_col_b = 68,
        light_ray_mult = 69,
        light_ray_underwater_mult = 70,
        light_ray_dist = 71,
        light_ray_heightfalloff = 72,
        light_ray_height_falloff_start = 73,
        light_ray_add_reducer = 74,
        light_ray_blit_size = 75,
        light_ray_length = 76,
        postfx_exposure = 77,
        postfx_exposure_min = 78,
        postfx_exposure_max = 79,
        postfx_bright_pass_thresh_width = 80,
        postfx_bright_pass_thresh = 81,
        postfx_intensity_bloom = 82,
        postfx_correct_col_r = 83,
        postfx_correct_col_g = 84,
        postfx_correct_col_b = 85,
        postfx_correct_cutoff = 86,
        postfx_shift_col_r = 87,
        postfx_shift_col_g = 88,
        postfx_shift_col_b = 89,
        postfx_shift_cutoff = 90,
        postfx_desaturation = 91,
        postfx_noise = 92,
        postfx_noise_size = 93,
        postfx_tonemap_filmic_override_dark = 94,
        postfx_tonemap_filmic_exposure_dark = 95,
        postfx_tonemap_filmic_a = 96,
        postfx_tonemap_filmic_b = 97,
        postfx_tonemap_filmic_c = 98,
        postfx_tonemap_filmic_d = 99,
        postfx_tonemap_filmic_e = 100,
        postfx_tonemap_filmic_f = 101,
        postfx_tonemap_filmic_w = 102,
        postfx_tonemap_filmic_override_bright = 103,
        postfx_tonemap_filmic_exposure_bright = 104,
        postfx_tonemap_filmic_a_bright = 105,
        postfx_tonemap_filmic_b_bright = 106,
        postfx_tonemap_filmic_c_bright = 107,
        postfx_tonemap_filmic_d_bright = 108,
        postfx_tonemap_filmic_e_bright = 109,
        postfx_tonemap_filmic_f_bright = 110,
        postfx_tonemap_filmic_w_bright = 111,
        postfx_vignetting_intensity = 112,
        postfx_vignetting_radius = 113,
        postfx_vignetting_contrast = 114,
        postfx_vignetting_col_r = 115,
        postfx_vignetting_col_g = 116,
        postfx_vignetting_col_b = 117,
        postfx_grad_top_col_r = 118,
        postfx_grad_top_col_g = 119,
        postfx_grad_top_col_b = 120,
        postfx_grad_middle_col_r = 121,
        postfx_grad_middle_col_g = 122,
        postfx_grad_middle_col_b = 123,
        postfx_grad_bottom_col_r = 124,
        postfx_grad_bottom_col_g = 125,
        postfx_grad_bottom_col_b = 126,
        postfx_grad_midpoint = 127,
        postfx_grad_top_middle_midpoint = 128,
        postfx_grad_middle_bottom_midpoint = 129,
        postfx_scanlineintensity = 130,
        postfx_scanline_frequency_0 = 131,
        postfx_scanline_frequency_1 = 132,
        postfx_scanline_speed = 133,
        postfx_motionblurlength = 134,
        dof_far = 135,
        dof_blur_mid = 136,
        dof_blur_far = 137,
        dof_enable_hq = 138,
        dof_hq_smallblur = 139,
        dof_hq_shallowdof = 140,
        dof_hq_nearplane_out = 141,
        dof_hq_nearplane_in = 142,
        dof_hq_farplane_out = 143,
        dof_hq_farplane_in = 144,
        environmental_blur_in = 145,
        environmental_blur_out = 146,
        environmental_blur_size = 147,
        bokeh_brightness_min = 148,
        bokeh_brightness_max = 149,
        bokeh_fade_min = 150,
        bokeh_fade_max = 151,
        nv_light_dir_mult = 152,
        nv_light_amb_down_mult = 153,
        nv_light_amb_up_mult = 154,
        nv_lowLum = 155,
        nv_highLum = 156,
        nv_topLum = 157,
        nv_scalerLum = 158,
        nv_offsetLum = 159,
        nv_offsetLowLum = 160,
        nv_offsetHighLum = 161,
        nv_noiseLum = 162,
        nv_noiseLowLum = 163,
        nv_noiseHighLum = 164,
        nv_bloomLum = 165,
        nv_colorLum_r = 166,
        nv_colorLum_g = 167,
        nv_colorLum_b = 168,
        nv_colorLowLum_r = 169,
        nv_colorLowLum_g = 170,
        nv_colorLowLum_b = 171,
        nv_colorHighLum_r = 172,
        nv_colorHighLum_g = 173,
        nv_colorHighLum_b = 174,
        hh_startRange = 175,
        hh_farRange = 176,
        hh_minIntensity = 177,
        hh_maxIntensity = 178,
        hh_displacementU = 179,
        hh_displacementV = 180,
        hh_tex1UScale = 181,
        hh_tex1VScale = 182,
        hh_tex1UOffset = 183,
        hh_tex1VOffset = 184,
        hh_tex2UScale = 185,
        hh_tex2VScale = 186,
        hh_tex2UOffset = 187,
        hh_tex2VOffset = 188,
        hh_tex1UFrequencyOffset = 189,
        hh_tex1UFrequency = 190,
        hh_tex1UAmplitude = 191,
        hh_tex1VScrollingSpeed = 192,
        hh_tex2UFrequencyOffset = 193,
        hh_tex2UFrequency = 194,
        hh_tex2UAmplitude = 195,
        hh_tex2VScrollingSpeed = 196,
        lens_dist_coeff = 197,
        lens_dist_cube_coeff = 198,
        chrom_aberration_coeff = 199,
        chrom_aberration_coeff2 = 200,
        lens_artefacts_intensity = 201,
        lens_artefacts_min_exp_intensity = 202,
        lens_artefacts_max_exp_intensity = 203,
        blur_vignetting_radius = 204,
        blur_vignetting_intensity = 205,
        screen_blur_intensity = 206,
        sky_zenith_transition_position = 207,
        sky_zenith_transition_east_blend = 208,
        sky_zenith_transition_west_blend = 209,
        sky_zenith_blend_start = 210,
        sky_zenith_col_r = 211,
        sky_zenith_col_g = 212,
        sky_zenith_col_b = 213,
        sky_zenith_col_inten = 214,
        sky_zenith_transition_col_r = 215,
        sky_zenith_transition_col_g = 216,
        sky_zenith_transition_col_b = 217,
        sky_zenith_transition_col_inten = 218,
        sky_azimuth_transition_position = 219,
        sky_azimuth_east_col_r = 220,
        sky_azimuth_east_col_g = 221,
        sky_azimuth_east_col_b = 222,
        sky_azimuth_east_col_inten = 223,
        sky_azimuth_transition_col_r = 224,
        sky_azimuth_transition_col_g = 225,
        sky_azimuth_transition_col_b = 226,
        sky_azimuth_transition_col_inten = 227,
        sky_azimuth_west_col_r = 228,
        sky_azimuth_west_col_g = 229,
        sky_azimuth_west_col_b = 230,
        sky_azimuth_west_col_inten = 231,
        sky_hdr = 232,
        sky_plane_r = 233,
        sky_plane_g = 234,
        sky_plane_b = 235,
        sky_plane_inten = 236,
        sky_sun_col_r = 237,
        sky_sun_col_g = 238,
        sky_sun_col_b = 239,
        sky_sun_disc_col_r = 240,
        sky_sun_disc_col_g = 241,
        sky_sun_disc_col_b = 242,
        sky_sun_disc_size = 243,
        sky_sun_hdr = 244,
        sky_sun_miephase = 245,
        sky_sun_miescatter = 246,
        sky_sun_mie_intensity_mult = 247,
        sky_sun_influence_radius = 248,
        sky_sun_scatter_inten = 249,
        sky_moon_col_r = 250,
        sky_moon_col_g = 251,
        sky_moon_col_b = 252,
        sky_moon_disc_size = 253,
        sky_moon_iten = 254,
        sky_stars_iten = 255,
        sky_moon_influence_radius = 256,
        sky_moon_scatter_inten = 257,
        sky_cloud_gen_frequency = 258,
        sky_cloud_gen_scale = 259,
        sky_cloud_gen_threshold = 260,
        sky_cloud_gen_softness = 261,
        sky_cloud_density_mult = 262,
        sky_cloud_density_bias = 263,
        sky_cloud_mid_col_r = 264,
        sky_cloud_mid_col_g = 265,
        sky_cloud_mid_col_b = 266,
        sky_cloud_base_col_r = 267,
        sky_cloud_base_col_g = 268,
        sky_cloud_base_col_b = 269,
        sky_cloud_base_strength = 270,
        sky_cloud_shadow_col_r = 271,
        sky_cloud_shadow_col_g = 272,
        sky_cloud_shadow_col_b = 273,
        sky_cloud_shadow_strength = 274,
        sky_cloud_gen_density_offset = 275,
        sky_cloud_offset = 276,
        sky_cloud_overall_strength = 277,
        sky_cloud_overall_color = 278,
        sky_cloud_edge_strength = 279,
        sky_cloud_fadeout = 280,
        sky_cloud_hdr = 281,
        sky_cloud_dither_strength = 282,
        sky_small_cloud_col_r = 283,
        sky_small_cloud_col_g = 284,
        sky_small_cloud_col_b = 285,
        sky_small_cloud_detail_strength = 286,
        sky_small_cloud_detail_scale = 287,
        sky_small_cloud_density_mult = 288,
        sky_small_cloud_density_bias = 289,
        cloud_shadow_density = 290,
        cloud_shadow_softness = 291,
        cloud_shadow_opacity = 292,
        dir_shadow_num_cascades = 293,
        dir_shadow_distance_multiplier = 294,
        dir_shadow_softness = 295,
        dir_shadow_cascade0_scale = 296,
        sprite_brightness = 297,
        sprite_size = 298,
        sprite_corona_screenspace_expansion = 299,
        Lensflare_visibility = 300,
        sprite_distant_light_twinkle = 301,
        water_reflection = 302,
        water_reflection_far_clip = 303,
        water_reflection_lod = 304,
        water_reflection_sky_flod_range = 305,
        water_reflection_lod_range_enabled = 306,
        water_reflection_lod_range_hd_start = 307,
        water_reflection_lod_range_hd_end = 308,
        water_reflection_lod_range_orphanhd_start = 309,
        water_reflection_lod_range_orphanhd_end = 310,
        water_reflection_lod_range_lod_start = 311,
        water_reflection_lod_range_lod_end = 312,
        water_reflection_lod_range_slod1_start = 313,
        water_reflection_lod_range_slod1_end = 314,
        water_reflection_lod_range_slod2_start = 315,
        water_reflection_lod_range_slod2_end = 316,
        water_reflection_lod_range_slod3_start = 317,
        water_reflection_lod_range_slod3_end = 318,
        water_reflection_lod_range_slod4_start = 319,
        water_reflection_lod_range_slod4_end = 320,
        water_reflection_height_offset = 321,
        water_reflection_height_override = 322,
        water_reflection_height_override_amount = 323,
        water_reflection_distant_light_intensity = 324,
        water_reflection_corona_intensity = 325,
        water_foglight = 326,
        water_interior = 327,
        water_fogstreaming = 328,
        water_foam_intensity_mult = 329,
        water_drying_speed_mult = 330,
        water_specular_intensity = 331,
        mirror_reflection_local_light_intensity = 332,
        fog_start = 333,
        fog_near_col_r = 334,
        fog_near_col_g = 335,
        fog_near_col_b = 336,
        fog_near_col_a = 337,
        fog_col_r = 338,
        fog_col_g = 339,
        fog_col_b = 340,
        fog_col_a = 341,
        fog_sun_lighting_calc_pow = 342,
        fog_moon_col_r = 343,
        fog_moon_col_g = 344,
        fog_moon_col_b = 345,
        fog_moon_col_a = 346,
        fog_moon_lighting_calc_pow = 347,
        fog_east_col_r = 348,
        fog_east_col_g = 349,
        fog_east_col_b = 350,
        fog_east_col_a = 351,
        fog_density = 352,
        fog_falloff = 353,
        fog_base_height = 354,
        fog_alpha = 355,
        fog_horizon_tint_scale = 356,
        fog_hdr = 357,
        fog_haze_col_r = 358,
        fog_haze_col_g = 359,
        fog_haze_col_b = 360,
        fog_haze_density = 361,
        fog_haze_alpha = 362,
        fog_haze_hdr = 363,
        fog_haze_start = 364,
        fog_shape_bottom = 365,
        fog_shape_top = 366,
        fog_shape_log_10_of_visibility = 367,
        fog_shape_weight_0 = 368,
        fog_shape_weight_1 = 369,
        fog_shape_weight_2 = 370,
        fog_shape_weight_3 = 371,
        fog_shadow_amount = 372,
        fog_shadow_falloff = 373,
        fog_shadow_base_height = 374,
        fog_volume_light_range = 375,
        fog_volume_light_fade = 376,
        fog_volume_light_intensity = 377,
        fog_volume_light_size = 378,
        fogray_contrast = 379,
        fogray_intensity = 380,
        fogray_density = 381,
        fogray_nearfade = 382,
        fogray_farfade = 383,
        reflection_lod_range_start = 384,
        reflection_lod_range_end = 385,
        reflection_slod_range_start = 386,
        reflection_slod_range_end = 387,
        reflection_interior_range = 388,
        reflection_tweak_interior_amb = 389,
        reflection_tweak_exterior_amb = 390,
        reflection_tweak_emissive = 391,
        reflection_tweak_directional = 392,
        reflection_hdr_mult = 393,
        far_clip = 394,
        temperature = 395,
        particle_emissive_intensity_mult = 396,
        vfxlightning_intensity_mult = 397,
        vfxlightning_visibility = 398,
        particle_light_intensity_mult = 399,
        natural_ambient_multiplier = 400,
        artificial_int_ambient_multiplier = 401,
        fog_cut_off = 402,
        no_weather_fx = 403,
        no_gpu_fx = 404,
        no_rain = 405,
        no_rain_ripples = 406,
        fogvolume_density_scalar = 407,
        fogvolume_density_scalar_interior = 408,
        fogvolume_fog_scaler = 409,
        time_offset = 410,
        vehicle_dirt_mod = 411,
        wind_speed_mult = 412,
        entity_reject = 413,
        lod_mult = 414,
        enable_occlusion = 415,
        enable_shadow_occlusion = 416,
        render_exterior = 417,
        portal_weight = 418,
        light_falloff_mult = 419,
        lodlight_range_mult = 420,
        shadow_distance_mult = 421,
        lod_mult_hd = 422,
        lod_mult_orphanhd = 423,
        lod_mult_lod = 424,
        lod_mult_slod1 = 425,
        lod_mult_slod2 = 426,
        lod_mult_slod3 = 427,
        lod_mult_slod4 = 428,
    }
}

