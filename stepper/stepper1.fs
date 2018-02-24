PB9 constant M1A
PB8 constant M2A
PB7 constant M3A
PB6 constant M4A

%11 constant motor-full-mask
create motor-full \ full steps
%1010 c,
%0110 c,
%0101 c,
%1001 c,

4 variable motor.delay
0 variable motor.phase
1 variable motor.direction

: motor-on ( -- )
  motor.phase @ motor-full + c@ \ read current step bits
  dup 1 and if M1A ios! else M1A ioc! then
  2/
  dup 1 and if M2A ios! else M2A ioc! then
  2/
  dup 1 and if M3A ios! else M3A ioc! then
  2/
      1 and if M4A ios! else M4A ioc! then
;

: motor-off ( -- )
  M1A ioc!
  M2A ioc!
  M3A ioc!
  M4A ioc!
;

: motor.init ( -- )
  omode-pp M1A io-mode!
  omode-pp M2A io-mode!
  omode-pp M3A io-mode!
  omode-pp M4A io-mode!
  motor-off
;

: motor-step ( -- )
  motor.phase @
  motor.direction @ + motor-full-mask and \ change phase`
  motor.phase ! \ update
  motor-on \ set output accordingly
;

: motor-reverse ( -- )
  motor.direction @
  0< if 1 else -1 then motor.direction !
;

: motor-print ( -- )
  motor.phase @ .
  M1A io@ .
  M2A io@ .
  M3A io@ .
  M4A io@ .
  cr
;

: motor-move ( n -- )
  0 do
    motor-step
    \ motor-print
    motor.delay @ ms
  loop
  motor-off
;

motor.init
