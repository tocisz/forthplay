: probe ( port -- )
  begin
    cr
    200 ms
    dup adc u.
  key? until
  drop ;

: probe-a-once ( -- )
  PA8 PA0 do
    i adc u. space
  loop ;

: probe-a ( -- )
  begin
    cr
    200 ms
    probe-a
  key? until
  drop ;
