PB1 constant ZCLK

: zclk-10Hz ( -- )  \ slow clock for debugging, 50% duty, 4 ns rise/fall
  10 ZCLK pwm-init
  5000 ZCLK pwm ;

omode-pp ZCLK io-mode!
