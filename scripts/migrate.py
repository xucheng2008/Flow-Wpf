#!/usr/bin/env python3
"""
Flow-Wpf Canvas to Nodify Migration Tool

Converts legacy Canvas-based flowchart files to the new Nodify format.

Usage:
    python migrate.py <input_file> <output_file>

Example:
    python migrate.py flows/old_flow.json flows/converted_flow.flow
"""

import json
import sys
from datetime import datetime
from pathlib import Path


def migrate_canvas_to_nodify(canvas_data: dict) -> dict:
    """
    Convert Canvas format to Nodify format.
    
    Canvas format (legacy):
    {
        "shapes": [...],
        "connections": [...]
    }
    
    Nodify format (new):
    {
        "version": "2.0",
        "title": "...",
        "nodes": [...],
        "connections": [...]
    }
    """
    nodify_data = {
        "version": "2.0",
        "title": canvas_data.get("title", "Migrated Flow"),
        "description": f"Migrated from Canvas format on {datetime.now().isoformat()}",
        "metadata": {
            "created": canvas_data.get("created", datetime.now().isoformat()),
            "modified": datetime.now().isoformat(),
            "migrated_from": "Canvas"
        },
        "nodes": [],
        "connections": []
    }
    
    # Migrate shapes to nodes
    shapes = canvas_data.get("shapes", [])
    for shape in shapes:
        node = {
            "id": shape.get("id", f"node_{len(nodify_data['nodes'])}"),
            "type": map_shape_to_node_type(shape.get("type", "rectangle")),
            "x": shape.get("x", 0),
            "y": shape.get("y", 0),
            "title": shape.get("text", shape.get("title", "Node")),
            "data": shape.get("data", {})
        }
        nodify_data["nodes"].append(node)
    
    # Migrate connections
    connections = canvas_data.get("connections", [])
    for conn in connections:
        connection = {
            "id": conn.get("id", f"conn_{len(nodify_data['connections'])}"),
            "from": conn.get("from", conn.get("source", "")),
            "to": conn.get("to", conn.get("target", "")),
            "label": conn.get("label", "")
        }
        nodify_data["connections"].append(connection)
    
    return nodify_data


def map_shape_to_node_type(shape_type: str) -> str:
    """Map Canvas shape types to Nodify node types."""
    mapping = {
        "start": "Start",
        "end": "End",
        "process": "Process",
        "rectangle": "Process",
        "decision": "Decision",
        "diamond": "Decision",
        "loop": "Loop",
        "for": "Loop",
        "while": "Loop"
    }
    return mapping.get(shape_type.lower(), "Process")


def migrate_file(input_path: str, output_path: str) -> bool:
    """Migrate a single file from Canvas to Nodify format."""
    try:
        # Read Canvas format
        with open(input_path, 'r', encoding='utf-8') as f:
            canvas_data = json.load(f)
        
        # Convert to Nodify format
        nodify_data = migrate_canvas_to_nodify(canvas_data)
        
        # Write Nodify format
        with open(output_path, 'w', encoding='utf-8') as f:
            json.dump(nodify_data, f, indent=2, ensure_ascii=False)
        
        print(f"✅ Successfully migrated:")
        print(f"   Input:  {input_path}")
        print(f"   Output: {output_path}")
        print(f"   Nodes:  {len(nodify_data['nodes'])}")
        print(f"   Connections: {len(nodify_data['connections'])}")
        
        return True
    
    except FileNotFoundError:
        print(f"❌ Error: Input file not found: {input_path}")
        return False
    
    except json.JSONDecodeError as e:
        print(f"❌ Error: Invalid JSON in {input_path}: {e}")
        return False
    
    except Exception as e:
        print(f"❌ Error: {e}")
        return False


def migrate_directory(input_dir: str, output_dir: str) -> dict:
    """Migrate all flow files in a directory."""
    input_path = Path(input_dir)
    output_path = Path(output_dir)
    output_path.mkdir(parents=True, exist_ok=True)
    
    stats = {
        "success": 0,
        "failed": 0,
        "total": 0
    }
    
    # Find all JSON files
    for json_file in input_path.glob("*.json"):
        stats["total"] += 1
        output_file = output_path / f"{json_file.stem}.flow"
        
        if migrate_file(str(json_file), str(output_file)):
            stats["success"] += 1
        else:
            stats["failed"] += 1
    
    return stats


def main():
    if len(sys.argv) < 2:
        print(__doc__)
        print("\nModes:")
        print("  Single file:  python migrate.py <input.json> <output.flow>")
        print("  Directory:    python migrate.py --dir <input_dir> <output_dir>")
        sys.exit(1)
    
    if sys.argv[1] == "--dir":
        if len(sys.argv) < 4:
            print("❌ Error: Directory mode requires input and output directories")
            sys.exit(1)
        
        stats = migrate_directory(sys.argv[2], sys.argv[3])
        print(f"\n📊 Migration Summary:")
        print(f"   Total:   {stats['total']}")
        print(f"   Success: {stats['success']}")
        print(f"   Failed:  {stats['failed']}")
    
    else:
        if len(sys.argv) < 3:
            print("❌ Error: Single file mode requires input and output files")
            sys.exit(1)
        
        success = migrate_file(sys.argv[1], sys.argv[2])
        sys.exit(0 if success else 1)


if __name__ == "__main__":
    main()
