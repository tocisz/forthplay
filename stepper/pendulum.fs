PB9 constant M1A
PB8 constant M2A
PB7 constant M3A
PB6 constant M4A

%11 constant motor.fullsteps-mask
create motor.fullsteps \ full steps
%1010 c,
%0110 c,
%0101 c,
%1001 c,

%111 constant motor.halfsteps-mask
create motor.halfsteps \ half steps
%1010 c,
%0010 c,
%0110 c,
%0100 c,
%0101 c,
%0001 c,
%1001 c,
%1000 c,

1 variable motor.mode \ 0 - full steps | 1 - half steps
: motor:mask ( -- mask )
  motor.mode @
  if   motor.halfsteps-mask
  else motor.fullsteps-mask
  then
;

: motor:step-bits ( n -- bits )
  motor.mode @
  if   motor.halfsteps
  else motor.fullsteps
  then + c@
;

2000 variable motor.delay
0 variable motor.phase

\ change working mode and correct phase number
: motor:set-mode ( mode -- )
  dup ( mode mode )
  2* motor.mode @ or ( mode new_mode|old_mode-as-bits )
  dup ( mode new_mode|old_mode-as-bits x2 )

  motor.phase @ swap ( mode new_mode|old_mode-as-bits phase new_mode|old_mode-as-bits )
  case
    %01 of 2/ endof \ half to full
    %10 of 2* endof \ full to half
  endcase motor.phase ! ( mode new_mode|old_mode-as-bits )

  motor.delay @ swap ( mode delay new_mode|old_mode-as-bits )
  case
    %01 of 2* endof \ half to full
    %10 of 2/ endof \ full to half
  endcase motor.delay ! ( mode )

  motor.mode !
;

: motor:on ( -- )
  motor.phase @ motor:step-bits \ read current step bits
  dup 1 and if M1A ios! else M1A ioc! then
  2/
  dup 1 and if M2A ios! else M2A ioc! then
  2/
  dup 1 and if M3A ios! else M3A ioc! then
  2/
      1 and if M4A ios! else M4A ioc! then
;

: motor:off ( -- )
  M1A ioc!
  M2A ioc!
  M3A ioc!
  M4A ioc!
;

: motor:init ( -- )
  omode-pp M1A io-mode!
  omode-pp M2A io-mode!
  omode-pp M3A io-mode!
  omode-pp M4A io-mode!
  motor:off
;

: motor:step ( direction -- )
  motor.phase @ + motor:mask and \ change phase`
  motor.phase ! \ update
  motor:on \ set output accordingly
;

: motor:print ( -- )
  motor.phase @ .
  M1A io@ .
  M2A io@ .
  M3A io@ .
  M4A io@ .
  cr
;

: motor:move ( n -- )
  dup 0< if -1 else 1 then ( n sign[n] )
  swap abs ( sign[n] |n| )
  0 do
    dup motor:step
    \ motor:print
    motor.delay @ us
  loop ( sign[n] )
  drop
  motor:off
;

\ Fast integer square root. Algorithm from the book "Hacker's Delight".
: sqrt ( u -- u^1/2 )
  [
  $2040 h, \   movs r0, #0x40
  $0600 h, \   lsls r0, #24
  $2100 h, \   movs r1, #0
  $000A h, \ 1:movs r2, r1
  $4302 h, \   orrs r2, r0
  $0849 h, \   lsrs r1, #1
  $4296 h, \   cmp r6, r2
  $D301 h, \   blo 2f
  $1AB6 h, \   subs r6, r2
  $4301 h, \   orrs r1, r0
  $0880 h, \ 2:lsrs r0, #2
  $D1F6 h, \   bne 1b
  $000E h, \   movs r6, r1
  ]
  1-foldable
;

\ calculate sqrt for s31,32 < 1
: 0sqrt ( d -- sqrt[d] )
  drop \ should be 0 anyway
  sqrt 16 lshift \ sqrt and correct point
  0 \ add integer part back
  1-foldable
;

\ 1 over sqrt ( 1 - (x/256)^2 )
: darctg ( n -- df )
  dup 0=
  if drop 1,0
  else
    0 swap \ convert integer to df
    256,0 f/ \ x = x/256
    2dup f* \ x = x^2
    1,0 2swap d- \ x = 1-x
    0sqrt \ x = sqrt(x)
    1,0 2swap f/ \ x = 1/x
  then
  1-foldable
;

: pendulum ( min-delay step -- delay )
  darctg
  rot 0 swap f*
  nip
  1-foldable
;

256 constant motor.move-profile-size \ 256 values plus counter
800 constant motor.min-delay

create motor.move-profile motor.move-profile-size 1+ 2* allot \ 16-bit values
: init-pendulum-profile ( -- )
  motor.move-profile-size motor.move-profile h! \ size
  0 \ counter
  motor.move-profile 2+
  dup motor.move-profile-size 1- 2* +
  do
    dup motor.min-delay swap pendulum i h!
    1+
  -2 +loop
  drop
;
init-pendulum-profile \ 11.8 ms

: print-profile ( -- )
  motor.move-profile h@ .
  motor.move-profile 2+
  dup motor.move-profile h@ 2* +
  swap do
    i h@ .
  2 +loop
;

: motor:moves ( n -- )
  dup 0< if -1 else 1 then >r ( n R: sign[n] )
  abs ( |n| )

  dup motor.move-profile h@ 2* > \ is it twice bigger ?
  if
    motor.move-profile h@ ( |n| 10 )
    swap motor.move-profile h@ 2* - ( 10 |n|-20  )
    over ( 10 |n|-20 10  )
  else
    dup 2/ ( |n| |n|/2  )
    swap 1 and ( |n|/2 |n|%2 )
    over ( |n|/2 |n|%2 |n|/2 )
  then
  r> ( min[10, n/2] |n|-2*min[10, n/2] min[10, n/2] sign[n] )

  \ speeding up
  swap ( min[10, n/2] |n|-2*min[10, n/2] sign[n] min[10, n/2] )
  2*
  motor.move-profile 2+ dup rot + \ base addr motor.move-profile +1 cell
  swap do
    dup motor:step
    i h@ us
  2 +loop ( sign[n] )

  \ constant speed
  swap ( min[10, n/2] sign[n] |n|-2*min[10, n/2] )
  0 ?do
    dup motor:step
    motor.min-delay us
  loop ( sign[n] )

  \ slowing down
  swap ( sign[n] min[10, n/2] )
  1- 2*
  motor.move-profile 2+ dup rot + \ base addr motor.move-profile +1 cell
  do
    dup motor:step
    i h@ us
  -2 +loop ( sign[n] )
  drop

  motor:off
;

motor:init

: do-pendulum ( -- )
  begin
    512 motor:moves
    -512 motor:moves
  key? until
;
