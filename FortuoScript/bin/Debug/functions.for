/complen {
len /s2 swap def
len /s1 swap def
s1 s2 = { "same length" } if
s1 s2 > { "first string is longer" } if
s1 s2 < { "second string is longer" } if
/s1 undef
/s2 undef
} def

/countdown {
	true { 
		dup . cr 
		1 - 
		dup 0 < not
	} while
} def

/fletcher {
	/str swap def
	/i 0 def
	/sum1 0 def
	/sum2 0 def

	i str len < {
		/sum1 sum1 str i getchar + 255 mod def
		/sum2 sum2 sum1 + 255 mod def
		/i i 1 + def 
		i str len <
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

/split {
	0 getchar /spchar swap def
	/str swap def
	/i 0 def
	/lastsp 0 def
	list
	true {
		str i getchar spchar =
		{
			str lastsp i lastsp - substr
			add
			/lastsp i 1 + def
		} if
		/i i 1 + def
		i str len <
	} while
	str lastsp i lastsp - substr add
	/spchar undef
	/str undef
	/i undef
	/lastsp undef
} def

"included functions.for" . cr cr