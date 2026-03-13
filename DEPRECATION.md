# ⚠️ DEPRECATION NOTICE

## Flow-Wpf Canvas Version - DEPRECATED

**Status**: ❌ Deprecated (as of 2026-03-13)  
**Successor**: ✅ Flow-Wpf.Nodify  
**Migration Deadline**: 2026-06-01

---

## 📢 What's Happening

The original Canvas-based Flow-Wpf editor is being deprecated in favor of the new **Nodify-based** version.

### Why Migrate to Nodify?

| Feature | Canvas | Nodify |
|---------|--------|--------|
| Performance | ⚠️ Slower with many nodes | ✅ Optimized for 100s of nodes |
| MVVM Support | ⚠️ Partial | ✅ Full MVVM from ground up |
| Themes | ❌ None | ✅ Dark/Light themes |
| Execution Engine | ❌ None | ✅ Built-in interpreter |
| Serialization | ⚠️ Custom format | ✅ Standard JSON |
| Active Development | ❌ No | ✅ Yes |

---

## 🔄 Migration Guide

### Option 1: Use Migration Tool

```bash
# Single file
python scripts/migrate.py flows/old_flow.json flows/new_flow.flow

# Batch migration
python scripts/migrate.py --dir flows/ flows-nodify/
```

### Option 2: Manual Recreation

1. Open Flow-Wpf.Nodify
2. Recreate nodes using the new templates
3. Save in .flow format

---

## 📅 Timeline

| Date | Event |
|------|-------|
| 2026-03-13 | Canvas version deprecated |
| 2026-04-01 | Nodify becomes default |
| 2026-06-01 | Canvas version archived |
| 2026-12-31 | Canvas version removed |

---

## 🆘 Need Help?

- **Documentation**: See `README.md` in Flow-Wpf.Nodify/
- **Migration Issues**: Open an issue on GitHub
- **Questions**: Contact the development team

---

## ✅ Benefits of Upgrading

- 🚀 **Better Performance** - Optimized for large flowcharts
- 🎨 **Modern UI** - Dark/Light themes
- 🔧 **Execution Engine** - Run your flowcharts
- 💾 **Standard Format** - JSON serialization
- 🧪 **Unit Tests** - Verified functionality
- 📖 **Active Development** - Regular updates

---

*Migrate today and experience the future of Flow-Wpf!*
