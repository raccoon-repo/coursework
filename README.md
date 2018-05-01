# coursework
This is my coursework on topic "Personal library". Such technologies as C#, MySql an other have been applied.

Core of the program is inside BookLibrary directory. 
It has got several interfaces for manipulating data, entities and some utils.

There are two implementations of those interfaces: 
  1. When data are stored in datatabase
  2. When data are stored in xml files

Implementations are separated into independent projects - class libraries.
So if I need to write GUI, I simply choose where data to be stored and then
use the suitable implementation
