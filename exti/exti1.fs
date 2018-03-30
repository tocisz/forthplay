include pwm.fs

$E000E100 constant NVIC-EN0R \ IRQ 0 to 31 Set Enable Register
$E000E104 constant NVIC-EN1R \ IRQ 32 to 63 Set Enable Register

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

create captured 1000 cells allot
captured 1000 cells + constant captured-end
0 variable capture-ptr

: capture-reset ( -- )
  captured capture-ptr !
;

: ext10-tick ( -- )  \ interrupt handler for EXTI10_15
  12 bit EXTI-PR !  \ clear interrupt
  1 intcnt +!
;

: ext10-capture
  12 bit EXTI-PR !  \ clear interrupt
  1 intcnt +!
  micros
  capture-ptr @ !
  capture-ptr @ 1 cells + \ next position
  dup captured-end = if \ at the end
    drop
    ['] ext10-tick irq-exti10 !
  else
    capture-ptr !
  then
;

: capture-print ( -- )
  captured-end captured do
    i @ .
  1 cells +loop
;

: capture-print-diff ( -- )
  captured @
  captured-end captured 1 cells + do
    i @ ( captured[i-1] captured[i] )
    dup rot ( captured[i] captured[i-1] captured[i] )
    - . ( captured[i] )
  1 cells +loop
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

rtc-init
count-pulses
