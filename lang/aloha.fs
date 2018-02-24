: HELLO    ." Hello " ;
: GOODBYE  ." Goodbye " ;
' HELLO VARIABLE 'aloha
: ALOHA    'aloha @ EXECUTE ;

: SAY  ' 'aloha ! ;
: COMING   ['] HELLO   'aloha ! ;
: GOING    ['] GOODBYE 'aloha ! ;
