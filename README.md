# 这可能是我的第一个项目！  
- - -  
## 用途  
仅用于节奏医生关卡。  
仅仅是为了方便事件计数，然后康康哪些人是**特效 不多**[^1]。  
加入了亿些花里胡哨的小玩意，增加一点趣味性（应该吧？
[^1]: https://space.bilibili.com/406743035
- - -
## 经历  
早期有做过一个比较全面的版本，但一看代码，真的是看不下去（  
所以打算重新写一遍，这次换了个更高效的算法<font color=#cccccc>（应该吧？）</font>  
- - -  
## 打算  
相较于上个大版本，我可能还有更多想法……  
* <u>内嵌字体文件？[^2]</u>  
* <u>界面做小一点？</u>  
* <u>赋予一些动画？</u>  
* <u>更改绘图方式？</u>
* <u>加点沙雕功能？</u>  

可能会很久，但我尽量做完吧  
[^2]: 已应用
- - -
## 当前
* ~成功解决了让我头疼的内嵌字体的问题~
* ~在打开像 **when_the_apple_is_bad.zip [^3]** 这样的巨量事件关卡文件时进展会非常慢（老问题，暂无动向）~
* 事件图标嵌入问题:~暂不能动态访问resx，所以想着用GIF来代替~ ~用精灵图解决了~ 还是拆成了一堆图标……
* 完成了UI的图标绘制
* **感谢圈师傅(@LittleCircleOO)以及其他群友对这个项目的帮助与鼓励！**
* 文件读取方式大改，~所需文件上传暂缓~
* 添加了多选显示方案
* 为窗体尺寸变化赋上了简单的缓动效果
* 完成了DPI适配
* 增加了文件缺失的判断
* 能够同一次启动多次打开文件了
* 少量增加了成就
* 右键清空一个板块所有统计项
* 更改了算法，效率大增（本机实测，读取 **when_the_apple_is_bad.zip [^3]** 只需20.24s）
* 更改了启动方式，现在不会一启动就打开文件了
* 将字体移至外部读取，解决了字体无法读取问题
* 添加了进度条
* 添加了左键反选功能
* 显示关卡名时能够转化Unicode了
* 优化文件读取时的内存占用
* 读取程序内存占用（暂时没地儿放，以及好像加了这玩意后程序启动得很慢了……
* 添加窗口关闭动画（或许该考虑取消窗口边框了？或许转Unicode没用了？

- - -
<font color=#cccccc size=1>Markdown好难（（（</font>  
[^3]: https://codex.rhythm.cafe/bad-appl-BSYqKCtoXzn.rdzip
