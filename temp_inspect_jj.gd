extends SceneTree

var _lines: Array[String] = []

func _init():
    var scene: PackedScene = load("res://BlenderAsset/export/jj.glb")
    if scene == null:
        _lines.append("failed to load scene")
    else:
        var root = scene.instantiate()
        _print_node(root, "")
    var file = FileAccess.open("res://jj_inspect.txt", FileAccess.WRITE)
    for line in _lines:
        file.store_line(line)
    file.close()
    quit()

func _print_node(node: Node, indent: String):
    _lines.append(indent + node.name + " : " + node.get_class())
    for child in node.get_children():
        _print_node(child, indent + "  ")
