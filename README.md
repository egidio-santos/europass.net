# Europass.Net

**!!!THIS IS A PRE-ALPHA VERSION USE AT YOUR OWN RISK!!!**

Europass.Net is a simple C#/.Net implementation of the Europass CV Layout.

You can use this library to:
  - Read/Write Europass XML Files
  - Extract XML Info from exported PDF Files
  - Generate a PDF based on the standard layout **(Requires Internet Connection)**

### Tech

This project was made based on the Europass XML Schema 3.3.0 and .Net Framework 4.6.1, although it can be easily downgraded to older versions, just pay attention to the dependencies.

### Dependencies

The core library has the following dependencies. More will be added later for other extensions.

| Plugin | README |
| ------ | ------ |
| Newtonsoft Json | https://github.com/JamesNK/Newtonsoft.Json |
| iTextSharp | https://github.com/itext/itextsharp |
|Europass XML Schema 3.3.0 | http://interop.europass.cedefop.europa.eu/data-model/xml-resources/ |

### Todos

 - Implement REST Api for Online PDF Generation
 - Write MORE Tests
 - Implement local Html Editor **Will be a separate project** (Fix some bugs of the original web editor) 

License
----

GNU


**Free Software, Go nuts!**
