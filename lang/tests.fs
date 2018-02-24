: (4times) >r 4 0 do j execute loop rdrop ; ( on gforth )

: (4times) >r 4 0 do k execute loop rdrop ; ( on mecrisp )

: (4times) 4 0 do dup execute loop drop ; ( on gforth )

: loop3 ( -- )
  2 0 do
    2 0 do
      2 0 do
        k . j . i .
        cr
      loop
    loop
  loop
;

: 4times:   ' postpone literal postpone (4times) ; immediate
