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
