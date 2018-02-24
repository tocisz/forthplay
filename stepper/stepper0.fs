PB9 constant M1A
PB8 constant M2A
PB7 constant M3A
PB6 constant M4A
PB5 constant M12EN
PB4 constant M34EN

4 variable motor.delay
0 variable motor.state

: motor-on ( -- )
  M12EN ios!
  M34EN ios!
;

: motor-off ( -- )
  M12EN ioc!
  M34EN ioc!
;

: motor.init ( -- )
  omode-pp M1A io-mode!
  omode-pp M2A io-mode!
  omode-pp M3A io-mode!
  omode-pp M4A io-mode!
  omode-pp M12EN io-mode!
  omode-pp M34EN io-mode!
  motor-off
  M1A ios!
  M2A ioc!
  M3A ios!
  M4A ioc!
;

: motor-step ( -- )
  motor.state @ dup
    if   M1A iox! M2A iox!
    else M3A iox! M4A iox!
  then
  not motor.state !
;

: motor-reverse ( -- )
  motor.state @ not motor.state !
;

: motor-move ( n -- )
  0 do
    motor-step
    motor.delay @ ms
  loop
;

motor.init
