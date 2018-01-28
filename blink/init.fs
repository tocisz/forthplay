\ Stripped-down version of init from Embello system
\ With Hackaday frills

: flashfree compiletoram? $10000 compiletoflash here - swap if compiletoram then ;
: ramfree  compiletoram? not flashvar-here compiletoram here - swap if compiletoflash then ;

: init ( -- )
  \ jtag-deinit     \ disable JTAG, we only need SWD
  72MHz           \ set clock
  1000 systick-hz \ set ms ticker

 \ requires LED.fs
 led.init

 ." INIT ok." cr
;
