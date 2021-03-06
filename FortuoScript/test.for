﻿%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Test program for Fortuo Interpreter
% created by David Cukrowicz
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% clear Stack and Dictionary
clear

% integer addition, subtraction, multiplication and division
15 21 + "15 + 21 = " . . cr
21 15 - "21 - 15 = " . . cr
3 6 * "3 * 6 = " . . cr
10 3 / "10 / 3 = " . . cr
10 3 mod "10 mod 3 = " . . cr

% string parsing and output
"Hello World!" . cr
"This is a \"sentence\" with quotes!" . cr

% stack alteration
10 20 dup pstack
+ swap pstack

% name definition
% define var1 as 10
/var1 10 def
% use var1
var1 dup + . cr
% redefine var1
/var1 20 def var1 . cr

% define function
/sq { dup * } def % squares number on top of stack
5 sq . cr

clear
"Test successful!!" . cr cr