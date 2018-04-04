adc-init
adc-calib

: cycle ( fn -- )
  begin
    cr
    20 ms
    dup execute
  key? until
  drop
;

: cycle-1arg ( arg fn -- )
  begin
    cr
    50 ms
    2dup execute
  key? until
  2drop
;

: probe ( port -- )
  adc u.
;

: probe-a ( -- )
  PA8 PA0 do
    i adc u. space
  loop
;

: probe-ad ( -- )
  PA0 adc PA1 adc - .
  PA2 adc PA3 adc - .
  PA4 adc PA5 adc - .
;

: probe-d
  PA0 adc PA1 adc - 1000 + 20 / 0 max
  0 ?do
    ." #"
  loop
;

\ PA0 ' probe cycle-1arg
' probe-ad cycle
