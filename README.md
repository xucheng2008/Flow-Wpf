# Flow-Wpf

WPF Canvas-based flowchart/diagram editor for industrial automation and process visualization.

## Features

- 🎨 **Node-based editor** - Create flowcharts with rectangles, diamonds, circles, and rounded rectangles
- 🔗 **Smart connections** - Bezier curves, orthogonal lines, and straight connections
- 🖱️ **Interactive** - Drag, drop, zoom, and pan with mouse gestures
- 🎯 **Industrial styling** - Designed for ECU calibration and automation process flows
- 📦 **MVVM architecture** - Clean separation of concerns, data-binding ready
- ↩️ **Undo/Redo** - Full command history support
- 📤 **Export** - Save to XML/JSON, export as image

## Architecture

```
FlowChartEditor/
├── Models/           # Data models (Node, Connection, FlowChart)
├── ViewModels/       # MVVM ViewModels
├── Views/            # XAML views and templates
├── Controls/         # Custom WPF controls
├── Services/         # Layout, path generation, undo/redo
└── Converters/       # Value converters for data binding
```

## Getting Started

### Requirements

- .NET 6.0 or later
- Visual Studio 2022 or JetBrains Rider

### Build

```bash
dotnet restore
dotnet build
```

### Run

```bash
dotnet run --project src/FlowChartEditor
```

## Node Types

| Type | Shape | Usage |
|------|-------|-------|
| Rectangle | ▭ | Process step |
| Diamond | ◇ | Decision/condition |
| Circle | ○ | Start/End point |
| Rounded Rectangle | ▭ | Input/Output |

## Connection Types

- **Bezier** - Smooth curved connections
- **Orthogonal** - Right-angle connections (default for flowcharts)
- **Straight** - Direct line connections

## Roadmap

- [ ] Core canvas implementation
- [ ] Node rendering and dragging
- [ ] Connection line rendering
- [ ] Undo/Redo system
- [ ] Auto-layout engine
- [ ] Export/Import (XML, JSON, Image)
- [ ] Property editor panel
- [ ] Themes and styling

## License

MIT License - See [LICENSE](LICENSE) for details.

## Acknowledgments

Inspired by industrial automation flowchart standards and WPF diagram libraries.
