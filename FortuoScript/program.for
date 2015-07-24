/complen {
len /s2 swap def
len /s1 swap def
s1 s2 eq { "same length" } if
s1 s2 gt { "first string is longer" } if
s1 s2 lt { "second string is longer" } if
/s1 undef
/s2 undef
} def

/countdown {
true { dup . cr 
  1 sub 
  dup 0 lt not
} while
} def

/fletcher {
	/str swap def
	/i 0 def
	/sum1 0 def
	/sum2 0 def

	i str len lt {
		/sum1 sum1 str i getchar add 255 mod def
		/sum2 sum2 sum1 add 255 mod def
		/i i 1 add def 
		i str len lt
	} while

	sum2 256 mul sum1 add
	/str undef
	/i undef
	/sum1 undef
	/sum2 undef
} def