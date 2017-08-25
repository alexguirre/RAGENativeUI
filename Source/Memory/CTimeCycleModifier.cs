namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Size = 48)]
    internal unsafe struct CTimeCycleModifier
    {
        [FieldOffset(0x0000)] public Mod.CSimpleArray Mods;

        [FieldOffset(0x0010)] public uint Name;
        [FieldOffset(0x0018)] public long unk18;
        [FieldOffset(0x0020)] public uint Flags;
        [FieldOffset(0x0024)] public int unk24;

        public Mod* GetUnusedModEntry(short increaseCountIfFull = 5)
        {
            if (Mods.Count == Mods.Size)
            {
                short newSize = unchecked((short)(Mods.Size + increaseCountIfFull));
                Mods.Size = newSize;
                Mod* newOffset = (Mod*)GameMemory.Allocator->Allocate(sizeof(Mod) * newSize, 16, 0);
                for (short i = 0; i < Mods.Count; i++)
                {
                    newOffset[i] = *Mods.Get(i);
                }
                GameMemory.Allocator->Free((IntPtr)Mods.Offset);
                Mods.Offset = newOffset;
            }

            short last = Mods.Count;
            Mods.Count++;
            return Mods.Get(last);
        }

        public void RemoveModEntry(short index)
        {
            for (short i = index; i < (Mods.Count - 1); i++)
            {
                Mods.Offset[i] = Mods.Offset[i + 1];
            }

            Mods.Count--;
        }

        // call after adding new mods, to maintain the array in the correct order
        public void SortMods()
        {
            // https://en.wikipedia.org/wiki/Bubble_sort
            short count = Mods.Count;
            for (short write = 0; write < count; write++)
            {
                for (short sort = 0; sort < count - 1; sort++)
                {
                    if (Mods.Get(sort)->ModType > Mods.Get(unchecked((short)(sort + 1)))->ModType)
                    {
                        Mod temp = Mods.Offset[unchecked((short)(sort + 1))];
                        Mods.Offset[sort + 1] = Mods.Offset[sort];
                        Mods.Offset[sort] = temp;
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = 12)]
        public struct Mod
        {
            [FieldOffset(0x0000)] public ModType ModType;
            [FieldOffset(0x0004)] public float Value1;
            [FieldOffset(0x0008)] public float Value2;

            [StructLayout(LayoutKind.Sequential)]
            public unsafe struct CSimpleArray
            {
                public Mod* Offset;
                public short Count;
                public short Size;

                public Mod* Get(short index)
                {
                    if (index >= Size)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), $"The size of this {nameof(CTimeCycleModifier)}.{nameof(Mod)}.{nameof(CSimpleArray)} is {Size}, the index {index} is out of range.");
                    }

                    return &Offset[index];
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CSimpleArrayPtr
        {
            public CTimeCycleModifier** Offset;
            public short Count;
            public short Size;

            public CTimeCycleModifier* Get(short index)
            {
                if (index >= Size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"The size of this {nameof(CTimeCycleModifier)}.{nameof(CSimpleArrayPtr)} is {Size}, the index {index} is out of range.");
                }

                return Offset[index];
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CSortedArray
        {
            [StructLayout(LayoutKind.Explicit, Size = 16)]
            public struct Entry
            {
                [FieldOffset(0x0000)] public uint Name;
                [FieldOffset(0x0008)] public CTimeCycleModifier* Modifier;
            }

            public Entry* Offset;
            public short Count;
            public short Size;

            public Entry* Get(short index)
            {
                if (index >= Size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"The size of this {nameof(CTimeCycleModifier)}.{nameof(CSortedArray)} is {Size}, the index {index} is out of range.");
                }

                return &Offset[index];
            }
        }

        public enum ModType : int
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

            sky_hdr = 232,

            dir_shadow_num_cascades = 293,
            dir_shadow_distance_multiplier = 294,
            dir_shadow_softness = 295,
            dir_shadow_cascade0_scale = 296,
            sprite_brightness = 297,
            sprite_size = 298,

            Lensflare_visibility = 300,

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

            water_reflection_height_override = 322,
            water_reflection_height_override_amount = 323,

            water_foglight = 326,
            water_interior = 327,
            water_fogstreaming = 328,
            water_foam_intensity_mult = 329,

            fog_start = 333,
            fog_near_col_r = 334,
            fog_near_col_g = 335,
            fog_near_col_b = 336,
            fog_near_col_a = 337,
            fog_col_r = 338,
            fog_col_g = 339,
            fog_col_b = 340,
            fog_col_a = 341,
            fog_moon_col_r = 343,
            fog_moon_col_g = 344,
            fog_moon_col_b = 345,
            fog_moon_col_a = 346,
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

            particle_light_intensity_mult = 399,
            natural_ambient_multiplier = 400,
            artificial_int_ambient_multiplier = 401,
            fog_cut_off = 402,
            no_weather_fx = 403,
            no_gpu_fx = 404,
            no_rain = 405,

            wind_speed_mult = 412,
            entity_reject = 413,
            lod_mult = 414,

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
}

