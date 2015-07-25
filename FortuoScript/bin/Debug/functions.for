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
true { 
  dup . cr 
  1 - 
  dup 0 lt not
} while
} def

/fletcher {
	/str swap def
	/i 0 def
	/sum1 0 def
	/sum2 0 def

	i str len lt {
		/sum1 sum1 str i getchar + 255 mod def
		/sum2 sum2 sum1 + 255 mod def
		/i i 1 + def 
		i str len lt
	} while

	sum2 256 * sum1 +
	/str undef
	/i undef
	/sum1 undef
	/sum2 undef
} def

/plist {
	/i 0 def
	count {
		i get . cr
		/i i 1 + def
	} repeat
	/i undef
} def

list "item1" add "item2" add "item3" add plist