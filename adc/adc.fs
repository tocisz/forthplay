adc-init
adc-calib

: cycle ( arg fn -- )
  begin
    cr
    200 ms
    dup execute
  key? until
  drop
;

: cycle-1arg ( arg fn -- )
  begin
    cr
    200 ms
    2dup execute
  key? until
  2drop
;

: probe1 ( port -- )
  adc u.
;

: probe ( port -- )
  ['] probe1 cycle-1arg
;

: probe-a1 ( -- )
  PA8 PA0 do
    i adc u. space
  loop
;

: probe-a ( port -- )
  ['] probe-a1 cycle
;
