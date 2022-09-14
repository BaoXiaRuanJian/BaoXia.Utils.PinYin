# BaoXia.Utils.PinYin
.Net 中文拼音库，提供了41450个常用汉字的常用拼音字典。

官方提供的拼音库，没有首选（最常用）拼音的信息，如：使用官方拼音库获取“无”字的拼音信息，默认（第一个）为“mo”，而不是“wu”。

利用了C#的编译特效，将全部汉字组合为一个超长的字符串，再通过超长的索引数组，构建出了只需要很小内存就可以完整使用的“常用拼音库”功能。

“小巧、常用”是整个库的核心追求。
