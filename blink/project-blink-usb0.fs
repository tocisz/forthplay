\ Define the pin.
PC13 constant led.pin ( for blue pill boards )
\ PA1 constant led.pin ( for hytiny boards )

\ set output mode, push-pull driver
: led.init   omode-pp led.pin io-mode! ;
: led.on     led.pin ioc! ; \ LED lights up when port low
: led.off    led.pin ios! ;
: led.toggle led.pin iox! ;

: led.simpleblink led.on 100 ms led.off 200 ms ;

led.init
led.off

task: blinktask
: blink&
    blinktask activate
    begin led.blink again
;
multitask

blink&
