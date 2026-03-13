# Flow-Wpf.Nodify

WPF 节点编辑器 - 基于 Nodify 库实现的可视化流程图编辑器

## 项目结构

```
Flow-Wpf.Nodify/
├── Models/           # 数据模型
├── ViewModels/       # MVVM 视图模型
├── Views/            # XAML 视图和模板
├── Converters/       # 值转换器
├── App.xaml          # 应用程序入口
└── MainWindow.xaml   # 主窗口
```

## 先决条件

- .NET 8.0 SDK
- Nodify v7.2.0 (NuGet)

## 构建和运行

```bash
dotnet restore
dotnet build
dotnet run
```

## 功能

- ✅ Nodify.Editor 集成
- ✅ 无限画布（平移/缩放）
- ✅ 5 种节点模板（Start, Decision, Process, Loop, End）
- ✅ MVVM 架构
- ✅ 网格对齐

## 开发进度

- [x] Phase 1: 项目搭建
- [x] Phase 2: 节点模板
- [ ] Phase 3: 连接样式
- [ ] Phase 4: 编辑器功能
- [ ] Phase 5: ViewModel 层
- [ ] Phase 6: 序列化
- [ ] Phase 7: 执行引擎
- [ ] Phase 8: 主题美化
- [ ] Phase 9: 测试文档
- [ ] Phase 10: 迁移

## 许可证

MIT
