extends SceneTree

func _init():
    inspect_scene("res://BlenderAsset/export/jj.glb", "E:/Gogot/road-armed/jj_report.txt")
    inspect_scene("res://BlenderAsset/export/pistol.glb", "E:/Gogot/road-armed/pistol_report.txt")
    quit()

func inspect_scene(scene_path: String, out_path: String):
    var lines: Array[String] = []
    lines.append("SCENE: %s" % scene_path)
    var packed: PackedScene = load(scene_path)
    if packed == null:
        lines.append("FAILED: could not load")
        write_lines(out_path, lines)
        return
    var root = packed.instantiate()
    walk(root, "", lines)
    write_lines(out_path, lines)

func walk(node: Node, indent: String, lines: Array[String]):
    lines.append("%s%s : %s" % [indent, node.name, node.get_class()])
    if node is AnimationPlayer:
        var ap := node as AnimationPlayer
        lines.append("%s  Animations: %s" % [indent, ", ".join(ap.get_animation_list())])
    if node is Skeleton3D:
        var sk := node as Skeleton3D
        var names: Array[String] = []
        for i in range(sk.get_bone_count()):
            names.append(sk.get_bone_name(i))
        lines.append("%s  Bones: %s" % [indent, ", ".join(names)])
    for child in node.get_children():
        walk(child, indent + "  ", lines)

func write_lines(path: String, lines: Array[String]):
    var file := FileAccess.open(path, FileAccess.WRITE)
    if file == null:
        return
    for line in lines:
        file.store_line(line)
    file.close()
