
# FlameScript

FlameScript is a highly experimental programming language.
The lexer and parser are written completely in C#.
There is also an experimental interpreter written in C# that can execute FlameScript
source code; this will provide interopability between a host application and FlameScript.
As of right now, the language itself does not support invoking methods on dynamic objects (tables) [See the language specifications]
but when that feature is implemented, you will be able to share object instances between .NET and FlameScript.
Eventually, you will be to compile to a variety of targets (for example, .NET IL bytecode, JavaScript, or NekoVM).

## Features

### Implemented

- Lexing and parsing from code to AST

### In Progress

- Execution through an interpreter

### Planned

- Compilation to VM code
- Execution in a VM context

## Language specifications/features

See the [language specifications](langspecs.md) page

## Acknowledgements

- [SimpleC](https://github.com/nrother/simple-c) (MIT Licensed)
    - Some of the original lexer and AST nodes, as well as general design flow inspiration