bits 64
default rel

stub_offset:  dq stub

stub_success:       dq 0x1111111111111111
stub_failed:        dq 0x2222222222222222
base_address:       dq 0x3333333333333333  ; the address where this stub was placed
keyboard_layout:    dq 0x4444444444444444

align 16
stub:
        test    al, al
        jnz     .success
        lea     rdx, [rbp + 0x190] ; rdx = sIcon*
        lea     rcx, [rbp + 0x330] ; rcx = const char* token
        call    parser
        test    al, al
        jnz      .success
        jmp     [stub_failed]
    .success:
        jmp     [stub_success]

max_icons: equ 4

struc sIcon 
    .id                 resb 64
    .iconList           resd max_icons
    .iconTypeList       resb max_icons
    .padding_54         resd 4
    .useIdAsMovieName   resb 1
    .padding_65         resb 3
endstruc


key_max_text_length:        equ 10
keyboard_layout_size:       equ 255
sizeof_sKeyboardLayoutKey:  equ 0x10

struc sKeyboardLayoutKey
    .icon               resd 1
    .text               resb key_max_text_length
endstruc

; params:
;   const char* token - rcx
;   sIcon* icon       - rdx
; return:
;   bool              - al
align 16
parser:
        push    rbx
        push    rdi
        push    rsi

        mov     rbx, rcx ; save the token ptr, rcx is modified by strlen

        call    strlen     ; strlen(token)

        ; alias for 'b_998' (plus symbol)
        cmp     eax, 1                  ; if (length == 1 && token[0] == '+') return plus symbol
        jne     .length_check
        cmp     byte [rbx + 0], '+'
        jne     .length_check
        mov     byte  [rdx + sIcon.iconTypeList], 'b'
        mov     dword [rdx + sIcon.iconList], 998
        mov     byte  [rdx + sIcon.useIdAsMovieName], 0
        jmp     .return_success         ; success: '+' alias

    .length_check:
        cmp     eax, 2                  ; if (length <= 2) return
        jle     .return_failed          ; failed: token too short

        xor     r8, r8  ; state = 0
        xor     r9, r9  ; icon index = 0
    .state_machine_loop:
        lea     rcx, [parser_states_table]
        mov     rcx, [rcx + r8 * 8]
        add     rcx, [base_address]
        jmp     rcx
    .state_machine_prefix:
        mov     cl, byte [rbx]      ; if (c != 't' && c != 'T' && c != 'b') return
        sub     cl, 0x54
        cmp     cl, 0x20
        ja      .return_failed
        movzx   ecx, cl
        mov     rax, 0x100004001
        bt      rax, rcx
        jae     .return_failed      ; failed: expected 't', 'T' or 'b'

        mov     cl, byte [rbx]      ; icon.iconTypeList[iconIndex] = c;
        mov     byte  [rdx + sIcon.iconTypeList + r9], cl
        mov     r8, parser_state_underscore_separator
        jmp     .state_machine_continue

    .state_machine_underscore_separator:
        cmp     byte [rbx], '_'
        jne     .return_failed      ; failed: expected underscore '_'
        mov     r8, parser_state_icon
        jmp     .state_machine_continue

    .state_machine_icon:
        cmp     byte [rdx + sIcon.iconTypeList + r9], 'b'
        jne     .state_machine_icon.t
    
    .state_machine_icon.b:
        mov     rcx, rbx
        call    parse_uint ; parse_uint(token)
        test    cl, cl  ; success?
        jz      .return_failed      ; failed: invalid uint

        lea     rbx, [rbx + rsi]    ; token += len
        mov     dword [rdx + sIcon.iconList + r9 * 4], eax ; iconList[r9] = value

        jmp     .state_machine_icon.end
    .state_machine_icon.t:
        mov     rcx, [keyboard_layout]
        cmp     rcx, 0                  ; if (keyboard_layout != null) ... else fallback
        je     .state_machine_icon.t.fallback

        xor     eax, eax                ; key = 0
    .state_machine_icon.t.kb_loop:
        cmp     dword [rcx + sKeyboardLayoutKey.icon], 1
        jg      .state_machine_icon.t.kb_loop.continue

        xor     rdi, rdi    ; text idx
    .state_machine_icon.t.kb_loop.text_loop:
        cmp     rdi, key_max_text_length
        jge     .state_machine_icon.t.kb_loop.text_matches
        cmp     byte [rcx + sKeyboardLayoutKey.text + rdi], 0
        je      .state_machine_icon.t.kb_loop.text_matches    ; while (rdi < max_tentLength && text[rdi] != 0)

        mov     sil, [rbx + rdi]
        cmp     byte [rcx + sKeyboardLayoutKey.text + rdi], sil ; if (text[rdi] != token[i + rdi]) continue else text_matches
        jne     .state_machine_icon.t.kb_loop.continue

        add     rdi, 1
        jmp     .state_machine_icon.t.kb_loop.text_loop
    .state_machine_icon.t.kb_loop.text_matches:
        lea     rbx, [rbx + rdi - 1]                        ; i += text_idx - 1;
        mov     dword [rdx + sIcon.iconList + r9 * 4], eax  ; iconList[r9] = key
        jmp .state_machine_icon.end

    .state_machine_icon.t.kb_loop.continue:
        lea     rcx, [rcx + sizeof_sKeyboardLayoutKey]  ; layout_key++
        add     eax, 1                                  ; key++
        cmp     eax, keyboard_layout_size
        jl      .state_machine_icon.t.kb_loop           ; while (key < keyboard_layout_size)

        jmp     .state_machine_icon.end
    .state_machine_icon.t.fallback: ; fallback, only valid for alphanumeric keys
        movzx   ecx, byte [rbx]
        mov     dword [rdx + sIcon.iconList + r9 * 4], ecx ; iconList[r9] = token[curr]
    .state_machine_icon.end:
        mov     r8, parser_state_group_separator
        jmp     .state_machine_continue

    .state_machine_group_separator:
        cmp     byte [rbx], '%'
        jne     .return_failed      ; failed: expected group separator '%'

        add     r9, 1               ; icon index++
        cmp     r9, max_icons
        jge     .return_failed      ; failed: too many icons

        mov     r8, parser_state_prefix
        jmp     .state_machine_continue

    .state_machine_continue:
        lea     rbx, [rbx + 1]      ; token++
        cmp     byte [rbx], 0       ; while (*token != 0)
        jne     .state_machine_loop

        mov     byte  [rdx + sIcon.useIdAsMovieName], 0
    .return_success:
        mov     al, 1
        jmp .epilog
    .return_failed:
        xor     al, al
    .epilog:
        pop     rsi
        pop     rdi
        pop     rbx
        ret

align 8
parser_states_table:
    dq parser.state_machine_prefix
    dq parser.state_machine_underscore_separator
    dq parser.state_machine_icon
    dq parser.state_machine_group_separator

parser_state_prefix:               equ 0
parser_state_underscore_separator: equ 1
parser_state_icon:                 equ 2
parser_state_group_separator:      equ 3

; params:
;   const char* str  - rcx
; return:
;   uint value       - eax
;   bool success     -  cl
;   uint read_length - rsi
parse_uint:
        push    rdx

        xor     eax, eax    ; value = 0
        xor     rsi, rsi    ; len = 0
    .loop:
        movzx   edx, byte [rcx] ; c = *str
        test    dl, dl
        je      .success
        cmp     dl, '%'              ; while (c != 0 && c != '%')
        je      .success
        add     eax, eax
        lea     eax, [rax + 4 * rax] ; value *= 10
        lea     edx, [edx - '0']
        cmp     dl, 9                ; if (c < '0' || c > '9') failed
        ja      .failed
        add     eax, edx            ; value += (*str - '0');
        add     rsi, 1              ; len++
        add     rcx, 1              ; str++
        jmp     .loop
    .failed:
        xor     cl, cl
        jmp     .exit
    .success:
        mov     cl, 1
    .exit:
        pop     rdx
        ret



; params:
;   const char* str - rcx
; return:
;   int             - eax
strlen:
        mov     eax, -1
    .loop:
        add     eax, 1
        cmp     byte [rcx], 0
        lea     rcx, [rcx + 1]
        jne     .loop
        ret