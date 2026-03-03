"""
Splits images/labels into train/val sets and writes dataset.yaml.
Run once before training.
"""
import os, shutil, random
from pathlib import Path

ROOT = Path(__file__).parent
IMAGES_SRC = ROOT / "images"
LABELS_SRC = ROOT / "labels"

CLASSES = ["Boom", "Bucket", "Cab", "Stick", "Wheels"]
VAL_SPLIT = 0.2
SEED = 42

# Collect samples that have both an image and a label
samples = []
for img in IMAGES_SRC.iterdir():
    label = LABELS_SRC / (img.stem + ".txt")
    if label.exists():
        samples.append(img.stem)

random.seed(SEED)
random.shuffle(samples)

n_val = max(1, int(len(samples) * VAL_SPLIT))
val_set = set(samples[:n_val])
train_set = set(samples[n_val:])

print(f"Total paired samples: {len(samples)}")
print(f"Train: {len(train_set)}  |  Val: {len(val_set)}")

for split in ("train", "val"):
    (ROOT / split / "images").mkdir(parents=True, exist_ok=True)
    (ROOT / split / "labels").mkdir(parents=True, exist_ok=True)

def copy_split(names, split):
    for stem in names:
        img_src = IMAGES_SRC / (stem + ".jpg")
        lbl_src = LABELS_SRC / (stem + ".txt")
        shutil.copy2(img_src, ROOT / split / "images" / img_src.name)
        shutil.copy2(lbl_src, ROOT / split / "labels" / lbl_src.name)

copy_split(train_set, "train")
copy_split(val_set, "val")

yaml_text = f"""path: {ROOT.as_posix()}
train: train/images
val: val/images

nc: {len(CLASSES)}
names: {CLASSES}
"""

yaml_path = ROOT / "dataset.yaml"
yaml_path.write_text(yaml_text)
print(f"\nWrote {yaml_path}")
print("Ready to train!")

# yolo detect train data=dataset.yaml model=yolov8n.pt epochs=100 imgsz=640 batch=8 project=runs name=excavator_v1
'''
  yolo detect predict \
  model=runs/excavator_v1/weights/best.pt \
  source=/path/to/your/images \
  conf=0.25 \
  save=True



python prepare_dataset.py
yolo detect train data=dataset.yaml model=yolov8n.pt epochs=100 imgsz=640 batch=8 project=runs name=excavator_v2
python convert_for_unity.py


'''