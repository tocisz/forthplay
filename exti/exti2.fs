$E000E000 constant NVIC

$E000E100 constant NVIC-EN0R \ IRQ 0 to 31 Set Enable Register
$E000E104 constant NVIC-EN1R \ IRQ 32 to 63 Set Enable Register
\ NVIC_ISER0
\ NVIC_ISER1
NVIC $14 + constant NVIC-SYSTICKRVR
NVIC $18 + constant NVIC-SYSTICKCVR

AFIO $08 + constant AFIO-EXTICR1
AFIO $0C + constant AFIO-EXTICR2
AFIO $10 + constant AFIO-EXTICR3
AFIO $14 + constant AFIO-EXTICR4

$40010400 constant EXTI
EXTI $00 + constant EXTI-IMR
EXTI $08 + constant EXTI-RTSR
EXTI $0C + constant EXTI-FTSR
EXTI $14 + constant EXTI-PR

0 variable intcnt

1000 constant captured.size
create captured captured.size cells allot
captured captured.size cells + constant captured.end
0 variable capture-ptr
create captured-val captured.size allot
0 variable capture-val-ptr

: capture-reset ( -- )
  captured capture-ptr !
  captured-val capture-val-ptr !
;

: ext10-tick ( -- )  \ interrupt handler for EXTI10_15
  12 bit EXTI-PR !  \ clear interrupt
  1 intcnt +!
;

: ext10-capture
  12 bit EXTI-PR !  \ clear interrupt
  1 intcnt +!
  PB12 io@ >r
  NVIC-SYSTICKCVR @
  capture-ptr @ !
  r> capture-val-ptr @ c!
  capture-ptr @ 1 cells + \ next position
  dup captured.end = if \ at the end
    drop
    ['] ext10-tick irq-exti10 !
  else
    capture-ptr !
    1 capture-val-ptr +!
  then
;

: capture-print ( -- )
  captured.end captured do
    i @ .
  1 cells +loop
;

: f.2 2 f.n ;

: capture-print-diff ( -- )
  captured @
  captured.end captured 1 cells + do
    i @ ( captured[i-1] captured[i] )
    dup -rot ( captured[i] captured[i-1] captured[i] )
    - ( captured[i]-captured[i] )
    dup 0< if NVIC-SYSTICKRVR @ 1+ + then \ correct if negative
    0 swap 72,0 f/ f.2
  1 cells +loop
;

: print ( -- )
  captured @
  captured.size 1- 0 do
    captured-val i + c@ if 1 else 0 then .

    captured i 1+ cells + @
    dup -rot ( captured[i] captured[i-1] captured[i] )
    - ( captured[i]-captured[i] )
    dup 0< if NVIC-SYSTICKRVR @ 1+ + then \ correct if negative
    0 swap 72,0 f/ f.2

    cr
  loop
;

: capture-val-print
  captured-val dup 1000 + swap do
    i c@ .
  loop
;

\ : test
\   capture-reset
\   1001 0 do capture-test loop
\ ;

: count-pulses ( -- )  \ set up and start the external interrupts
       ['] ext10-tick irq-exti10 !     \ install interrupt handler EXTI 10-15

       8 bit NVIC-EN1R bis!  \ enable EXTI15_10 interrupt 40
    %0001 AFIO-EXTICR4 bis!  \ select P<B>12
       12 bit EXTI-IMR bis!  \ enable PB<12>
       12 bit EXTI-RTSR bis!  \ trigger on PB<12> rising edge
       12 bit EXTI-FTSR bis!  \ trigger on PB<12> falling edge
;

: capture-pulses ( -- )
  capture-reset
  ['] ext10-capture irq-exti10 !
;

: count1s ( -- cnt )
  intcnt @
  1000 ms
  intcnt @
  swap -
;

\ rtc-init
count-pulses
