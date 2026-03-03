"""
Exports best.pt and reshapes the ONNX output to match
the 3-output format the Meta XR Building Blocks expects:
  output_0: (8400, 4)  float  - bounding boxes
  output_1: (8400,)    int64  - class indices
  output_2: (8400,)    float  - confidence scores

Run: python convert_for_unity.py
Then drag best_metaxr.onnx into your Unity Assets folder.
"""
import subprocess, sys
subprocess.run([sys.executable, "-m", "pip", "install", "onnx"], check=True)

from ultralytics import YOLO
from onnx import helper, TensorProto
from pathlib import Path
import onnx

NUM_CLASSES = 5  # Boom, Bucket, Cab, Stick, Wheels
ROOT        = Path(__file__).parent
RUNS_DIR    = ROOT / 'runs'

# Auto-detect the latest excavator_vN run
versions = sorted(RUNS_DIR.glob('excavator_v*'), key=lambda p: int(p.name.split('_v')[-1]))
if not versions:
    raise FileNotFoundError("No excavator_vN runs found in runs/")
latest = versions[-1]
version_num = latest.name.split('_v')[-1]
PT_PATH  = str(latest / 'weights' / 'best.pt')
ONNX_OUT = str(ROOT / f'best_metaxr_v{version_num}.onnx')
print(f"Using: {latest.name}  ->  best_metaxr_v{version_num}.onnx")

# Step 1: export to standard ONNX  ->  output shape [1, 9, 8400]
print("Exporting best.pt to ONNX...")
YOLO(PT_PATH).export(format='onnx', imgsz=640, opset=12, simplify=True)
src_onnx = str(Path(PT_PATH).parent / 'best.onnx')

# Step 2: load and rewrite the output nodes
print("Reshaping outputs...")
m = onnx.load(src_onnx)
g = m.graph
raw = g.output[0].name   # [1, 9, 8400]
del g.output[:]

def add_const(name, vals):
    g.initializer.append(helper.make_tensor(name, TensorProto.INT64, [len(vals)], vals))

# [1, 9, 8400] -> [9, 8400]  (drop batch dim)
g.node.append(helper.make_node('Squeeze', [raw], ['sq'], axes=[0]))

# boxes: rows 0-4  ->  [4, 8400]  ->  transpose  ->  [8400, 4]
add_const('b_start', [0]); add_const('b_end', [4]); add_const('b_ax', [0])
g.node.append(helper.make_node('Slice', ['sq','b_start','b_end','b_ax'], ['bx_ch']))
g.node.append(helper.make_node('Transpose', ['bx_ch'], ['output_0'], perm=[1, 0]))

# scores: rows 4 to 4+nc  ->  [nc, 8400]
add_const('s_start', [4]); add_const('s_end', [4 + NUM_CLASSES]); add_const('s_ax', [0])
g.node.append(helper.make_node('Slice', ['sq','s_start','s_end','s_ax'], ['sc_ch']))

# class index = argmax of scores  ->  [8400]  int64
g.node.append(helper.make_node('ArgMax', ['sc_ch'], ['output_1'], axis=0, keepdims=0))

# confidence = max of scores  ->  [8400]  float
g.node.append(helper.make_node('ReduceMax', ['sc_ch'], ['output_2'], axes=[0], keepdims=0))

# register the 3 outputs
g.output.extend([
    helper.make_tensor_value_info('output_0', TensorProto.FLOAT, [8400, 4]),
    helper.make_tensor_value_info('output_1', TensorProto.INT64, [8400]),
    helper.make_tensor_value_info('output_2', TensorProto.FLOAT, [8400]),
])

onnx.save(m, ONNX_OUT)
print(f"\nDone! Drag this file into Unity Assets:")
print(f"  {ONNX_OUT}")
print("\nIn Unity, set class labels to:")
print("  0: Boom  1: Bucket  2: Cab  3: Stick  4: Wheels")
