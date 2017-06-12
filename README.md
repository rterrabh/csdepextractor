# csdepextractor
It extracts all dependencies of a C# system into a txt file.

Usage (get the last release in [dist] directory:
> csdepextractor [folder-dir]

For example, if the *csdepextractor* is in the root source:
> csdepextractor .

It creates the *dependencies.txt* file in which **each** line is as follows:
> [source-class-full-qualified-name] , [dependency-type] , [target-class-full-qualified-name]

For example:
> com.test.ClassA , access , Console

PS: The dependency type can be: access, declare, create, extend, implement, useannotation, and throw.
