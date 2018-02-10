( Test that can be executed without any additional words loaded )
4 variable cnt

: cycle: ( arg fn -- )
  '
  4 cnt !
  begin
    cr
    dup execute
    -1 cnt +!
  cnt @ 0 = until
  drop
;

: test ." t" ;

cycle: test
( but is it possible to use "cycle: test" in definition? )
( see https://stackoverflow.com/questions/48510998/is-it-possible-to-consume-tick-in-forth-definition )
( unfortunately it seems not to work with mecrisp )
