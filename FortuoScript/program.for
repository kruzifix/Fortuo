
/complen {
len /s2 swap def
len /s1 swap def
s1 s2 eq { "same length" } if
s1 s2 gt { "first string is longer" } if
s1 s2 lt { "second string is longer" } if
/s1 undef
/s2 undef
} def

"kruzifix" "fortuo" complen . cr
"fortuo" "kruzifix" complen . cr
"fortuo" "fortuo" complen . cr