%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Test program for Fortuo Interpreter
% created by David Cukrowicz
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% clear Stack
clear

% integer addition, subtraction, multiplication and division
15 21 add "15 + 21 = " . . cr
21 15 sub "21 - 15 = " . . cr
3 6 mul "3 * 6 = " . . cr
10 3 div "10 / 3 = " . . cr
10 3 mod "10 mod 3 = " . . cr

% string parsing and output
"Hello World!" . cr
"This is a \"sentence\" with quotes!" . cr

% stack alteration
10 20 dup pstack
add exch pstack

% name definition
% define var1 as 10
/var1 10 def
% use var1
var1 dup add . cr
% redefine var1
/var1 20 def var1 . cr
