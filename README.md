# Flow-Wpf

基于 WPF 的可视化流程图编辑器，支持 Nodify 节点编辑引擎。

## 📦 项目结构

```
Flow-Wpf/
├── Flow-Wpf.Nodify/          # Nodify 版本（当前开发）
│   ├── Models/               # 数据模型
│   ├── ViewModels/           # MVVM 视图模型
│   ├── Views/                # XAML 视图和模板
│   ├── Services/             # 服务层（序列化、执行引擎）
│   ├── Themes/               # 主题资源
│   ├── Converters/           # 值转换器
│   └── MainWindow.xaml       # 主窗口
├── Flow-Wpf.Nodify.Tests/    # 单元测试
└── docs/                     # 文档
```

## 🚀 快速开始

### 先决条件

- .NET 8.0 SDK 或更高版本
- Visual Studio 2022（推荐）或 VS Code

### 构建和运行

```bash
# 克隆仓库
git clone https://github.com/xucheng2008/Flow-Wpf.git
cd Flow-Wpf/Flow-Wpf.Nodify

# 还原 NuGet 包
dotnet restore

# 构建项目
dotnet build

# 运行应用
dotnet run
```

### 运行测试

```bash
cd Flow-Wpf.Nodify.Tests
dotnet test
```

## ✨ 功能特性

### 节点编辑器

- ✅ **无限画布** - 平移、缩放（0.1x - 5.0x）
- ✅ **5 种节点类型** - Start, Process, Decision, Loop, End
- ✅ **直角连接** - CircuitConnection 样式
- ✅ **连接标签** - 支持 yes/no 分支标签
- ✅ **网格对齐** - 20px 点状网格
- ✅ **框选功能** - 多选节点

### MVVM 架构

- ✅ **完整数据绑定** - ViewModels 与 Views 分离
- ✅ **命令系统** - AddNode, DeleteNode, Save, Open 等
- ✅ **属性通知** - INotifyPropertyChanged

### 流程图执行

- ✅ **执行引擎** - FlowExecutor 解释执行
- ✅ **条件评估** - Decision 节点分支（SPEED < 200）
- ✅ **循环支持** - Loop 节点迭代
- ✅ **单步调试** - Step-through 执行
- ✅ **事件系统** - NodeExecuted, FlowCompleted

### 文件操作

- ✅ **JSON 序列化** - System.Text.Json
- ✅ **保存/加载** - .flow / .json 格式
- ✅ **版本控制** - 元数据支持

### UI 主题

- ✅ **深色主题** - 默认（Background #1E1E1E）
- ✅ **浅色主题** - 可选（Background #F5F5F5）
- ✅ **工具栏** - New, Open, Save, Execute, Zoom 等
- ✅ **状态栏** - 节点数、连接数、缩放比例

## 📖 使用指南

### 创建流程图

1. **添加节点** - 点击 "Add Node" 按钮
2. **移动节点** - 右键拖拽
3. **连接节点** - 从节点连接器拖拽到目标节点
4. **删除节点** - 选中后点击 "Delete"

### 执行流程图

1. 点击 "▶️ Execute" 开始执行
2. 使用 "⏭️ Step" 单步调试
3. 使用 "⏸️ Pause" 暂停执行
4. 使用 "⏹️ Stop" 停止执行

### 保存/加载

1. **保存** - 点击 "💾 Save"，选择 .flow 文件
2. **打开** - 点击 "📂 Open"，选择已有流程图
3. **新建** - 点击 "📄 New" 清空当前流程图

## 🏗️ 架构设计

### 核心类

| 类 | 命名空间 | 用途 |
|------|----------|------|
| NodeViewModel | ViewModels | 节点数据 + 属性通知 |
| ConnectionViewModel | ViewModels | 连接数据 + 标签 |
| MainViewModel | ViewModels | 主视图模型 + Commands |
| FlowExecutor | Services | 流程图执行引擎 |
| FlowSerializer | Services | JSON 序列化服务 |
| FlowData | Services | 序列化数据模型 |

### 节点类型

```csharp
public enum NodeType
{
    Start,      // 起始节点（圆角矩形，深色）
    Process,    // 处理节点（矩形，白色）
    Decision,   // 决策节点（菱形，浅蓝色）
    Loop,       // 循环节点（带徽章，橙色）
    End         // 结束节点（圆角矩形，深色）
}
```

### JSON Schema

```json
{
  "version": "1.0",
  "title": "Flow Chart",
  "nodes": [
    {
      "id": "abc123",
      "type": "Start",
      "x": 100,
      "y": 100,
      "title": "Start"
    }
  ],
  "connections": [
    {
      "id": "xyz789",
      "from": "abc123",
      "to": "def456",
      "label": "yes"
    }
  ]
}
```

## 🧪 测试

### 单元测试覆盖

- ✅ ViewModels 测试 - 节点 CRUD、命令
- ✅ FlowExecutor 测试 - 执行逻辑、条件评估
- ✅ Serialization 测试 - 保存/加载、往返

### 运行测试

```bash
dotnet test --verbosity normal
```

## 📝 开发笔记

### 添加新节点类型

1. 在 `NodeType` 枚举中添加新类型
2. 在 `NodeTemplates.xaml` 中创建 DataTemplate
3. 在 `NodeTypeToTemplateConverter` 中添加映射

### 自定义执行逻辑

```csharp
public class CustomExecutor : FlowExecutor
{
    protected override NodeViewModel? ExecuteProcessNode(NodeViewModel node)
    {
        // 自定义处理逻辑
        return base.ExecuteProcessNode(node);
    }
}
```

## 🔗 参考资源

- [Nodify GitHub](https://github.com/miroiu/nodify)
- [Nodify Wiki](https://github.com/miroiu/nodify/wiki)
- [Nodify NuGet](https://www.nuget.org/packages/Nodify)
- [.NET 8 文档](https://docs.microsoft.com/dotnet/core/whats-new/dotnet-8)

## 📄 许可证

MIT License

## 👥 贡献

欢迎提交 Issue 和 Pull Request！

---

*最后更新：2026-03-13*
