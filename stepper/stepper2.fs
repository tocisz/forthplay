PB9 constant M1A
PB8 constant M2A
PB7 constant M3A
PB6 constant M4A

%11 constant motor-fullsteps-mask
create motor-fullsteps \ full steps
%1010 c,
%0110 c,
%0101 c,
%1001 c,

%111 constant motor-halfsteps-mask
create motor-halfsteps \ half steps
%1010 c,
%0010 c,
%0110 c,
%0100 c,
%0101 c,
%0001 c,
%1001 c,
%1000 c,

0 variable motor.mode \ 0 - full steps | 1 - half steps
: motor-mask ( -- mask )
  motor.mode @
  if   motor-halfsteps-mask
  else motor-fullsteps-mask
  then
;

: motor-step-bits ( n -- bits )
  motor.mode @
  if   motor-halfsteps + c@
  else motor-fullsteps + c@
  then
;

4 variable motor.delay
0 variable motor.phase

\ change working mode and correct phase number
: motor-set-mode ( mode -- )
  dup ( mode mode )
  2* motor.mode @ or ( mode new_mode|old_mode-as-bits )
  case
    %01 of motor.phase @ 2/ motor.phase ! endof \ half to full
    %10 of motor.phase @ 2* motor.phase ! endof \ full to half
  endcase
  motor.mode !
;

: motor-on ( -- )
  motor.phase @ motor-step-bits \ read current step bits
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

: motor-step ( direction -- )
  motor.phase @ + motor-mask and \ change phase`
  motor.phase ! \ update
  motor-on \ set output accordingly
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
  dup 0< if -1 else 1 then ( n sign[n] )
  swap abs ( sign[n] |n| )
  0 do
    dup motor-step
    \ motor-print
    motor.delay @ ms
  loop ( sign[n] )
  drop
  motor-off
;

motor.init
