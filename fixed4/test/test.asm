section .text
fixed4_STR_1:
db 'b: %d, ',0

fixed4_STR_0:
db 'a: %d, ',0


extern _printf

global _tes
_tes:
push ebp
mov ebp, esp

push dword [ebp+8]

push fixed4_STR_0
call _printf
add esp, 8

push dword [ebp+12]

push fixed4_STR_1
call _printf
add esp, 8
pop ebp
ret
global _main
_main:
push ebp
mov ebp, esp
mov eax, 14
mov dword [ebp-4], eax

push 7

push dword [ebp-4]
call _tes
add esp, 8
pop ebp
ret