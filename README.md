# DesignerProtect
auto save photoshop,sai,sa2 editing files,only save *.psd file

# 实现原理
# PhotoShop部分
- 使用PhotoshopCS6 的开发库**PhotoshopTypeLibrary**和**PhotoshopObjectLibrary**提供的API直接对PhotoShop操作
- 入口是`app=newPhotoShop.ApplicationClass.ApplicationClass()`
- 调用 `app.Documents[index].SaveAs()`将打开的文档另存到备份目录
- PhotoShop的库文件在Ps安装目录下：
    - <Ps安装目录>\TypeLibrary.tlb\TypeLibrary.tlb
    - <Ps安装目录>\Required\Plug-Ins\Extensions\ScriptingSupport.8li

#sai && sai2
- 利用Process.GetProcessesByName获取进程号
- 使用EsayHook库Hook sai和sai2的一下WinApi调用
	 - MoveFileA
	 - GetFileAttributesA
	 - GetFileAttributesW
	 - MoveFileW
	 - CreateFileW
- 使用Win32 `PostMessage`接口定时向sai和sai2发送Ctrl+S消息
```
//向窗口发送Ctrl+S消息
IntPtr CTRL_KEY = new IntPtr(0x11);
uint KEY_DOWN = 0x0100;
uint KEY_UP = 0x0101;
IntPtr S_KEY = new IntPtr(0x53);
Win32.PostMessage(mainWindowHandle, KEY_DOWN, CTRL_KEY, IntPtr.Zero);
Win32.PostMessage(mainWindowHandle, KEY_DOWN, S_KEY, IntPtr.Zero);
Win32.PostMessage(mainWindowHandle, KEY_UP, S_KEY, IntPtr.Zero);
Win32.PostMessage(mainWindowHandle, KEY_UP, CTRL_KEY, IntPtr.Zero);
```
- sai&&sai2在保存时会调用WinApi的MoveFile接口，sai调用MoveFileA，sai2调用MoveFileW
- 在检测到MoveFile时，将刚保存的文件，拷贝到备份目录
