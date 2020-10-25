bits 64
default rel

stub_offset:  dq stub

stub_success:       dq 0x1111111111111111   ; return address used when the embedded texture is found
stub_failed:        dq 0x2222222222222222   ; return address to continue searching for the texture in fwTxdStore
fragment_store:     dq 0x3333333333333333
drawable_store:     dq 0x4444444444444444
get_hash_key:       dq 0x5555555555555555   ; uint(*)(const char* str, uint startHash)
; debug:              dq 0x6666666666666666

align 16
stub:   ; rcx = texture dictionary
        ; rdx = texture name
        push rcx
        push rdx    ; save texture names

        call find_embedded_texture
        
        pop rdx     ; restore texture names
        pop rcx

        test    rax, rax
        jz     .failed          ; if (texture == null) continue_searching else return texture
        jmp     [stub_success]
    .failed:
        mov     rdi, rdx        ; execute the instructions replaced by the jump hook before jumping back
        mov     r8, rcx
        jmp     [stub_failed]

; params
;   const char* texture_dictionary  - rcx
;   const char* texture_name        - rdx
; returns
;   rage::grcTexture* texture       - rax
align 16
find_embedded_texture:
        sub     rsp, 0x38

        ; if (texture_dictionary startsWith 'embed:') search else return null
        xor     rax, rax
%macro  check_char 2
        cmp     byte [rcx + %1], %2
        jne     .exit
%endmacro

        check_char 0, 'e'
        check_char 1, 'm'
        check_char 2, 'b'
        check_char 3, 'e'
        check_char 4, 'd'
        check_char 5, ':'

%unmacro check_char 2

        cmp     byte [rcx + 6], 0
        je      .exit

        lea     rcx, [rcx + 6] ; trim 'embed:'
        mov     qword [rsp+0x30], rdx
        mov     qword [rsp+0x28], rcx

        call    find_fragment_embedded_texture
        test    rax, rax
        jnz     .exit

        mov     rcx, qword [rsp+0x28]
        mov     rdx, qword [rsp+0x30]
        call    find_drawable_embedded_texture
    .exit:
        add     rsp, 0x38
        ret


fragType_PrimaryDrawable:       equ 0x30
rmcDrawableBase_ShaderGroup:    equ 0x10
grmShaderGroup_Textures:        equ 0x8
pgDictionary_Keys:              equ 0x20
pgDictionary_Values:            equ 0x30
atArray_Items:                  equ 0x0
atArray_Count:                  equ 0x8

fwAssetStore_FindSlot:          equ 0x10
fwAssetStore_GetPtr:            equ 0x40

; params
;   const char* texture_dictionary  - rcx
;   const char* texture_name        - rdx
; returns
;   rage::grcTexture* texture       - rax
align 16
find_fragment_embedded_texture:
        push    rbx
        sub     rsp, 0x40
        ; rsp + 0x38  = strLocalIndex
        ; rsp + 0x30  = texture_dictionary
        ; rsp + 0x28  = texture_name
        mov     qword [rsp + 0x30], rcx
        mov     qword [rsp + 0x28], rdx

        xor     rax, rax
        mov     rbx, [fragment_store] ; rbx = fwAssetStore<fragType>
        test    rbx, rbx
        jz      .exit

        mov     rax, [rbx]                              ; get vtable
        mov     rcx, rbx                                ; fragment_store
        lea     rdx, qword [rsp + 0x38]                 ; &index
        mov     r8, qword [rsp + 0x30]                  ; texture_dictionary
        call    qword [rax + fwAssetStore_FindSlot]     ; FindSlot(fragment_store, &index, texture_dictionary)

        xor     rax, rax
        cmp     dword [rsp + 0x38], -1                  ; if (index == -1) return null;
        je      .exit

        mov     rax, [rbx]                              ; get vtable
        mov     rcx, rbx                                ; fragment_store
        mov     edx, dword [rsp + 0x38]                 ; index
        call    qword [rax + fwAssetStore_GetPtr]       ; fragType* GetPtr(fragment_store, index)
        test    rax, rax
        jz      .exit                                   ; if (fragType == null) return null;

        ; we found the fragType, search for the textures in its drawables
        mov     rbx, rax                ; rbx = fragType

        mov     rcx, qword [rsp + 0x28] ; texture_name
        xor     rdx, rdx                ; startHash = 0
        call    qword [get_hash_key]

        mov     rcx, qword [rbx + fragType_PrimaryDrawable]     ; drawable
        mov     edx, eax                                        ; texture_name_hash
        call    search_texture_in_drawable                      ; search_texture_in_drawable(drawable, texture_name_hash)

        ; TODO: search in the other drawables that fragType may have?

        ; mov     rbx, rax
        ; mov     rcx, rax
        ; call    qword [debug]
        ; mov     rax, rbx

    .exit:
        add     rsp, 0x40
        pop     rbx
        ret

align 16
find_drawable_embedded_texture:
        push    rbx
        sub     rsp, 0x40
        ; rsp + 0x38  = strLocalIndex
        ; rsp + 0x30  = texture_dictionary
        ; rsp + 0x28  = texture_name
        mov     qword [rsp + 0x30], rcx
        mov     qword [rsp + 0x28], rdx

        xor     rax, rax
        mov     rbx, [drawable_store] ; rbx = fwAssetStore<rmcDrawable>
        test    rbx, rbx
        jz      .exit

        mov     rax, [rbx]                              ; get vtable
        mov     rcx, rbx                                ; drawable_store
        lea     rdx, qword [rsp + 0x38]                 ; &index
        mov     r8, qword [rsp + 0x30]                  ; texture_dictionary
        call    qword [rax + fwAssetStore_FindSlot]     ; FindSlot(drawable_store, &index, texture_dictionary)

        xor     rax, rax
        cmp     dword [rsp + 0x38], -1                  ; if (index == -1) return null;
        je      .exit

        mov     rax, [rbx]                              ; get vtable
        mov     rcx, rbx                                ; drawable_store
        mov     edx, dword [rsp + 0x38]                 ; index
        call    qword [rax + fwAssetStore_GetPtr]       ; rmcDrawable* GetPtr(drawable_store, index)
        test    rax, rax
        jz      .exit                                   ; if (drawable == null) return null;

        ; we found the drawable, search it for the textures
        mov     rbx, rax                ; rbx = rmcDrawable

        mov     rcx, qword [rsp + 0x28] ; texture_name
        xor     rdx, rdx                ; startHash = 0
        call    qword [get_hash_key]

        mov     rcx, rbx                        ; drawable
        mov     edx, eax                        ; texture_name_hash
        call    search_texture_in_drawable      ; search_texture_in_drawable(drawable, texture_name_hash)

    .exit:
        add     rsp, 0x40
        pop     rbx
        ret


; params
;   rmcDrawableBase* drawable       - rcx
;   uint texture_name_hash          - edx
; returns
;   rage::grcTexture* texture       - rax
align 16
search_texture_in_drawable:
        xor     rax, rax
        test    rcx, rcx
        jz      .exit

        mov     rcx, qword [rcx + rmcDrawableBase_ShaderGroup]
        mov     rcx, qword [rcx + grmShaderGroup_Textures]
        lea     r9, qword [rcx + pgDictionary_Keys]
        mov     r8, qword [r9 + atArray_Items]

        ; keys are sorted so binary search could be used, but I don't think any drawable will have enough textures to make it worthy, so just do linear search
    .loop: ; rax = i
        cmp     ax, word [r9 + atArray_Count] ; while (i < keys.Count)
        jge     .exit


        cmp     edx, dword [r8 + rax * 4]       ; if (texture_name_hash == keys[i]) found else continue
        je      .found

        add     rax, 1  ; i++
        jmp     .loop

    .not_found:
        xor     rax, rax
        jmp     .exit
    .found:
        lea     r9, qword [rcx + pgDictionary_Values]
        mov     r8, qword [r9 + atArray_Items]
        mov     rax, qword [r8 + rax * 8]       ; return values[i]

    .exit:
        ret
