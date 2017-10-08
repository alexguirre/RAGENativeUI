namespace RAGENativeUI
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;

    using Rage;

    using RAGENativeUI.Memory;

    // defined in timecycle_mods_*.xml
    /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Doc/*' />
    public unsafe sealed class TimeCycleModifier : IAddressable
    {
        private readonly uint hash;
        private readonly IntPtr memAddress;
        private readonly int index = -1;

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="Hash"]/*' />
        public uint Hash { get { return hash; } }
        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="Name"]/*' />
        public string Name
        {
            get
            {
                if (KnownNames.TimeCycleModifiers.Dictionary.TryGetValue(hash, out string n))
                {
                    return n;
                }

                return $"0x{hash:X8}";
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="IsActive"]/*' />
        public bool IsActive
        {
            get
            {
                return GameMemory.TimeCycleModifiersManager->CurrentModifierIndex == index || 
                       GameMemory.TimeCycleModifiersManager->TransitionModifierIndex == index;
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

        public bool IsInTransition
        {
            get { return GameMemory.TimeCycleModifiersManager->TransitionModifierIndex == index; }
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
            Throw.IfNull(name, nameof(name));
            Throw.IfNull(mods, nameof(mods));

            uint hash = Game.GetHashKey(name);

            Throw.InvalidOperationIf(GameMemory.TimeCycleModifiersManager->IsNameUsed(hash), $"The name '{name}' is already in use.");

            KnownNames.TimeCycleModifiers.Dictionary[hash] = name;

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
            : this(name, flags, mods?.Select(m => new CTimeCycleModifier.Mod { ModType = (int)m.Item1, Value1 = m.Item2, Value2 = m.Item3 }).ToArray())
        {
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="IsValid"]/*' />
        public bool IsValid()
        {
            return memAddress != IntPtr.Zero;
        }
        
        public void SetActiveWithTransition(float time) => SetActiveWithTransition(time, Strength);
        public void SetActiveWithTransition(float time, float targetStrength)
        {
            CTimeCycleModifiersManager* mgr = GameMemory.TimeCycleModifiersManager;
            mgr->CurrentModifierIndex = -1;
            mgr->CurrentModifierStrength = targetStrength;
            mgr->TransitionCurrentStrength = 0.0f;
            mgr->TransitionModifierIndex = Index;
            mgr->TransitionSpeed = targetStrength / time;
        }


        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="GetByName"]/*' />
        public static TimeCycleModifier GetByName(string name)
        {
            Throw.IfNull(name, nameof(name));

            uint hash = Game.GetHashKey(name);
            KnownNames.TimeCycleModifiers.Dictionary[hash] = name;
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
            Throw.IfOutOfRange(index, 0, NumberOfTimeCycleModifiers - 1, nameof(index));

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
                int index = GameMemory.TimeCycleModifiersManager->CurrentModifierIndex;
                if(index == -1)
                {
                    index = GameMemory.TimeCycleModifiersManager->TransitionModifierIndex;
                }
                return index == -1 ? null : GetByIndex(index);
            }
            set
            {
                if(value == null || !value.IsValid())
                {
                    GameMemory.TimeCycleModifiersManager->CurrentModifierIndex = -1;

                    GameMemory.TimeCycleModifiersManager->TransitionModifierIndex = -1;
                    GameMemory.TimeCycleModifiersManager->TransitionCurrentStrength = 0.0f;
                    GameMemory.TimeCycleModifiersManager->TransitionSpeed = 0.0f;
                }
                else
                {
                    GameMemory.TimeCycleModifiersManager->CurrentModifierIndex = value.Index;
                }
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifier/Member[@name="Strength"]/*' />
        public static float Strength
        {
            get
            {
                return GameMemory.TimeCycleModifiersManager->CurrentModifierStrength;
            }
            set
            {
                GameMemory.TimeCycleModifiersManager->CurrentModifierStrength = value;
            }
        }

        private static Dictionary<uint, TimeCycleModifier> cache = new Dictionary<uint, TimeCycleModifier>();

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

                Throw.IfOutOfRange(index, 0, native->Mods.Count - 1, nameof(index));

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
                    Throw.InvalidOperationIfNot(m.IsValid, $"This {nameof(TimeCycleModifierModsCollection)} doesn't contain a mod of type {type}.");
                    return m;
                }
                else
                {
                    Throw.InvalidOperationIf(GetIndexForType(type) == -1, $"This {nameof(TimeCycleModifierModsCollection)} doesn't contain a mod of type {type}.");
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
            Throw.InvalidOperationIf(Has(type), $"This {nameof(TimeCycleModifierModsCollection)} already contains a mod of type {type}.");

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

            Throw.InvalidOperationIf(index == -1, $"This {nameof(TimeCycleModifierModsCollection)} doesn't contain a mod of type {type}.");

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
            get { Throw.InvalidOperationIfNot(IsValid); return GetNative()->Value1; }
            set
            {
                Throw.InvalidOperationIfNot(IsValid);
                GetNative()->Value1 = value;
            }
        }

        /// <include file='..\Documentation\RAGENativeUI.TimeCycleModifier.xml' path='D/TimeCycleModifierMod/Member[@name="Value2"]/*' />
        public float Value2
        {
            get { Throw.InvalidOperationIfNot(IsValid); return GetNative()->Value2; }
            set
            {
                Throw.InvalidOperationIfNot(IsValid);
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

            Throw.IfOutOfRange(index, 0, native->Mods.Count - 1, nameof(index), "Shouldn't happen, notify a RAGENativeUI developer.");

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

