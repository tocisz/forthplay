adc-init
adc-calib

( Apostrophe in definition of this word reads address from word following )
( in invocation. )
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

: probe ( port -- )
  adc u.
;

: probe-a ( -- )
  PA8 PA0 do
    i adc u. space
  loop
;

PA0 ' probe cycle-1arg
' probe-a cycle
