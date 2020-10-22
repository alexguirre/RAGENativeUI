bits 64
default rel

stub_offset:  dq stub

stub_success:       dq 0x1111111111111111   ; return address used when the embedded texture is found
stub_failed:        dq 0x2222222222222222   ; return address to continue searching for the texture in fwTxdStore

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
        mov     rdi, rdx
        mov     r8, rcx
        jmp     [stub_failed]

; params
;   const char* texture_dictionary  - rcx
;   const char* texture_name        - rdx
; returns
;   rage::grcTexture* texture       - rax
align 16
find_embedded_texture:
        ; TODO: implement find_embedded_texture
        xor     rax, rax
        ret
